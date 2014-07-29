using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class ActualizeDealsDuringWithdrawalOperationService : IActualizeDealsDuringWithdrawalOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealActualizeDealProfitIndicatorsAggregateService _dealActualizeDealProfitIndicatorsAggregateService;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ActualizeDealsDuringWithdrawalOperationService(
            IDealReadModel dealReadModel,
            IDealActualizeDealProfitIndicatorsAggregateService dealActualizeDealProfitIndicatorsAggregateService,
            IDealChangeStageAggregateService dealChangeStageAggregateService,
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _dealReadModel = dealReadModel;
            _dealActualizeDealProfitIndicatorsAggregateService = dealActualizeDealProfitIndicatorsAggregateService;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Actualize(IEnumerable<long> dealIds)
        {
            _logger.InfoEx("Starting actualizing deals during withdrawal process");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeDealsDuringWithdrawalIdentity>())
            {
                var dealInfos = _dealReadModel.GetInfoForWithdrawal(dealIds);
                _dealActualizeDealProfitIndicatorsAggregateService.Actualize(dealInfos.Select(i => i.ActualizeProfitInfo));

                _dealChangeStageAggregateService.ChangeStage(
                    dealInfos
                        .Where(i => i.ActualizeProfitInfo.Deal.DealStage != (int)DealStage.Service && i.HasInactiveLocks)
                        .Select(i => new DealChangeStageDto
                            {
                                Deal = i.ActualizeProfitInfo.Deal,
                                NextStage = DealStage.Service
                            }));

                scope.Complete();
            }

            _logger.InfoEx("Finished actualizing deals during withdrawal process");
        }
    }
}