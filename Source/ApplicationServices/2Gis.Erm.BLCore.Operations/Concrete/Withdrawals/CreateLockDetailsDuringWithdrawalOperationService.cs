using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public class CreateLockDetailsDuringWithdrawalOperationService : ICreateLockDetailsDuringWithdrawalOperationService
    {
        private readonly IBulkCreateLockDetailsAggregateService _bulkCreateLockDetailsAggregateService;
        private readonly ICalculateOrderPositionCostService _calculateOrderPositionCostService;
        private readonly IWithdrawalInfoReadModel _withdrawalReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public CreateLockDetailsDuringWithdrawalOperationService(IWithdrawalInfoReadModel withdrawalReadModel,
                                                                 ICalculateOrderPositionCostService calculateOrderPositionCostService,
                                                                 IBulkCreateLockDetailsAggregateService bulkCreateLockDetailsAggregateService,
                                                                 IOperationScopeFactory scopeFactory)
        {
            _withdrawalReadModel = withdrawalReadModel;
            _calculateOrderPositionCostService = calculateOrderPositionCostService;
            _bulkCreateLockDetailsAggregateService = bulkCreateLockDetailsAggregateService;
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

                    lockDetailsToCreate.Add(new CreateLockDetailDto
                        {
                            Lock = orderPosition.Lock,
                            Amount = calculationResult.PayablePlan,
                            OrderPositionId = orderPosition.OrderPositionInfo.OrderPositionId,
                            PriceId = orderPosition.OrderPositionInfo.PriceId,
                            ChargeSessionId = orderPosition.ChargeInfo.SessionId
                        });
                }

                _bulkCreateLockDetailsAggregateService.Create(lockDetailsToCreate);

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