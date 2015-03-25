using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    // TODO {all, 30.05.2014}: есть мысль убрать создание LockDetails из списания, оставивь его только в одном месте - пока это finishrelease
    // Общий смысл - finishrelease, остается точно такой же как и до НМП, все блокировки со всеми LockDetails создаются в один момент момент через один и 
    // тот же AggregateService (для всех позиций и с гаратированным размещением и с негарантированным)
    // Перед списанием нужно просто вычитывать доп информацию по каждому LockDetails и связанной с ним позиции заказа - если размещение не гарантированное, 
    // и фактически отразмещавшаяся позиция отличается от заказанной - просто для таких позиций корректируем сумму LockDetails
    public sealed class CreateLockDetailsDuringWithdrawalOperationService : ICreateLockDetailsDuringWithdrawalOperationService
    {
        private readonly IAccountBulkCreateLockDetailsAggregateService _accountBulkCreateLockDetailsAggregateService;
        private readonly IPaymentsDistributor _paymentsDistributor;        
        private readonly IChargeReadModel _withdrawalReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICostCalculator _costCalculator;
        private readonly IOrderReadModel _orderReadModel;

        public CreateLockDetailsDuringWithdrawalOperationService(IChargeReadModel withdrawalReadModel,
                                                                 IAccountBulkCreateLockDetailsAggregateService accountBulkCreateLockDetailsAggregateService,
                                                                 IPaymentsDistributor paymentsDistributor,
                                                                 IOperationScopeFactory scopeFactory,
                                                                 ICostCalculator costCalculator,
                                                                 IOrderReadModel orderReadModel)
        {
            _withdrawalReadModel = withdrawalReadModel;
            _accountBulkCreateLockDetailsAggregateService = accountBulkCreateLockDetailsAggregateService;
            _paymentsDistributor = paymentsDistributor;
            _scopeFactory = scopeFactory;
            _costCalculator = costCalculator;
            _orderReadModel = orderReadModel;
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
                    var vatRateDetails = _orderReadModel.GetVatRateDetails(orderPosition.OrderInfo.SourceOrganizationUnitId, orderPosition.OrderInfo.DestOrganizationUnitId);

                    // За стоимость оказанной услуги принимается стоимость позиции за первый период размещения (т.е. теоритически меньшая величина, 
                    // чем за последний период в которой добавляются копейки для защиты от ошибок округления)
                    var calculationResult =
                        _costCalculator.Calculate(new CalcPositionRequest
                                                      {
                                                          Amount = 1,
                                                          CalculateDiscountViaPercent = true,
                                                          DiscountPercent = orderPosition.OrderPositionInfo.DiscountPercent,
                                                          PriceCost = orderPosition.ChargeInfo.Amount,
                                                          Rate = orderPosition.OrderPositionInfo.CategoryRate,
                                                          ReleaseCount = orderPosition.OrderInfo.ReleaseCountFact,
                                                          ShowVat = vatRateDetails.ShowVat,
                                                          VatRate = vatRateDetails.VatRate,
                                                      });

                    var paymentForSingleDistributionSlot = _paymentsDistributor.DistributePayment(orderPosition.OrderInfo.ReleaseCountFact,
                                                                                                      calculationResult.PayablePlan)
                                                                                   .First();

                    lockDetailsToCreate.Add(new CreateLockDetailDto
                                                {
                                                    Lock = orderPosition.Lock,
                                                    Amount = Math.Min(paymentForSingleDistributionSlot, orderPosition.OrderPositionInfo.AmountToWithdraw),
                                                    OrderPositionId = orderPosition.OrderPositionInfo.OrderPositionId,
                                                    PriceId = orderPosition.OrderPositionInfo.PriceId,
                                                    ChargeSessionId = orderPosition.ChargeInfo.SessionId
                                                });
                }

                _accountBulkCreateLockDetailsAggregateService.Create(lockDetailsToCreate);

                scope.Complete();
            }
        }

        private bool IsActual(IReadOnlyDictionary<long, Guid?> actualCharges, long projectId, Guid chargeSessionId)
        {
            Guid? actualId;
            return actualCharges.TryGetValue(projectId, out actualId) && actualId == chargeSessionId;
        }
    }
}