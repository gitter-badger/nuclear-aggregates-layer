﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class CreateLockDetailsDuringWithdrawalOperationService : ICreateLockDetailsDuringWithdrawalOperationService
    {
        private readonly IAccountBulkCreateLockDetailsAggregateService _accountBulkCreateLockDetailsAggregateService;
        private readonly IPaymentsDistributor _paymentsDistributor;
        private readonly ICalculateOrderPositionCostService _calculateOrderPositionCostService;
        private readonly IWithdrawalInfoReadModel _withdrawalReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public CreateLockDetailsDuringWithdrawalOperationService(IWithdrawalInfoReadModel withdrawalReadModel,
                                                                 ICalculateOrderPositionCostService calculateOrderPositionCostService,
                                                                 IAccountBulkCreateLockDetailsAggregateService accountBulkCreateLockDetailsAggregateService,
                                                                 IPaymentsDistributor paymentsDistributor,
                                                                 IOperationScopeFactory scopeFactory)
        {
            _withdrawalReadModel = withdrawalReadModel;
            _calculateOrderPositionCostService = calculateOrderPositionCostService;
            _accountBulkCreateLockDetailsAggregateService = accountBulkCreateLockDetailsAggregateService;
            _paymentsDistributor = paymentsDistributor;
            _scopeFactory = scopeFactory;
        }

        public void CreateLockDetails(long organizationUnitId, TimePeriod period)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CreateLockDetailsDuringWithdrawalIdentity>())
            {
                var actualCharges = _withdrawalReadModel.GetActualChargesByProject(period);

                // TODO {all, 23.05.2014}: Проверка отключена - https://jira.2gis.ru/browse/ERM-4092
                //var projectsWithoutCharges = actualCharges.Where(x => x.Value == null).Select(x => x.Key).ToArray();
                //if (projectsWithoutCharges.Any())
                //{
                //    throw new MissingChargesForProjectException(
                //        string.Format("Can't create lock details before withdrawing. The following projects have no charges: {0}.",
                //                      string.Join(", ", projectsWithoutCharges)));
                //}

                var plannedOrderPositionsWithCharges = _withdrawalReadModel.GetPlannedOrderPositionsWithChargesInfo(organizationUnitId, period);

                var orderPositionsWithoutCharges = plannedOrderPositionsWithCharges.Where(x => x.ChargeInfo == null).ToArray();
                if (orderPositionsWithoutCharges.Any())
                {
                    throw new MissingChargesForPlannedPositionsException(
                        string.Format("Can't create lock details before withdrawing. The following order positions have no charges: {0}.",
                                      string.Join(", ", orderPositionsWithoutCharges.Select(x => x.OrderPositionInfo.OrderPositionId))));
                }

                var inactualCharges =
                    plannedOrderPositionsWithCharges.Where(x => !IsActual(actualCharges, x.ChargeInfo.ProjectId, x.ChargeInfo.SessionId)).ToArray();
                if (inactualCharges.Any())
                {
                    throw new InactualChargesForPlannedPositionsException(
                        string.Format("Can't create lock details before withdrawing. The following order positions have inactual charges: {0}.",
                                      string.Join(", ", orderPositionsWithoutCharges.Select(x => x.OrderPositionInfo.OrderPositionId))));
                }

                var lockDetailsToCreate = new List<CreateLockDetailDto>();
                foreach (var orderPosition in plannedOrderPositionsWithCharges)
                {
                    decimal paymentForSingleDistributionSlot;

                    // если приобретенная позиция совпадает с фактически предоставленной, то используем для списание сумму из 
                    // releasewithdrawals (т.е. также как и для позиций с гарантированным размещением),
                    // иначе расчитываем стоимость оказанной фактически услуги.
                    // За стоимость оказанной услуги принимается стоимость позиции за первый период размещения (т.е. теоритически меньшая величина, 
                    // чем за последний период в которой добавляются копейки для защиты от ошибок округления)
                    if (IsPurchasedAndFactPositionAreEqual(orderPosition))
                    {
                        paymentForSingleDistributionSlot = orderPosition.OrderPositionInfo.AmountToWithdraw;
                    }
                    else
                    {
                        var calculationResult = _calculateOrderPositionCostService
                                                    .CalculateOrderPositionCostWithRate(orderPosition.OrderInfo.OrderType,
                                                                                        orderPosition.OrderInfo.ReleaseCountFact,
                                                                                        orderPosition.ChargeInfo.PositionId,
                                                                                        orderPosition.OrderPositionInfo.PriceId,
                                                                                        orderPosition.OrderPositionInfo.CategoryRate,
                                                                                        orderPosition.OrderPositionInfo.Amount,
                                                                                        orderPosition.OrderInfo.SourceOrganizationUnitId,
                                                                                        orderPosition.OrderInfo.DestOrganizationUnitId,
                                                                                        orderPosition.OrderPositionInfo.DiscountSum,
                                                                                        orderPosition.OrderPositionInfo.DiscountPercent,
                                                                                        orderPosition.OrderPositionInfo.CalculateDiscountViaPercent);

                        paymentForSingleDistributionSlot = _paymentsDistributor.DistributePayment(orderPosition.OrderInfo.ReleaseCountFact,
                                                                                                  calculationResult.PayablePlan)
                                                                               .First();
                    }
                    

                    lockDetailsToCreate.Add(new CreateLockDetailDto
                        {
                            Lock = orderPosition.Lock,
                            Amount = paymentForSingleDistributionSlot,
                            OrderPositionId = orderPosition.OrderPositionInfo.OrderPositionId,
                            PriceId = orderPosition.OrderPositionInfo.PriceId,
                            ChargeSessionId = orderPosition.ChargeInfo.SessionId
                        });
                }

                _accountBulkCreateLockDetailsAggregateService.Create(lockDetailsToCreate);

                scope.Complete();
            }
        }

        private bool IsPurchasedAndFactPositionAreEqual(OrderPositionWithChargeInfoDto positionWithChargeInfoDto)
        {
            return positionWithChargeInfoDto.OrderPositionInfo.PurchasedPositionId == positionWithChargeInfoDto.ChargeInfo.PositionId;
        }

        private bool IsActual(IReadOnlyDictionary<long, Guid?> actualCharges, long projectId, Guid chargeSessionId)
        {
            Guid? actualId;
            return actualCharges.TryGetValue(projectId, out actualId) && actualId == chargeSessionId;
        }
    }
}