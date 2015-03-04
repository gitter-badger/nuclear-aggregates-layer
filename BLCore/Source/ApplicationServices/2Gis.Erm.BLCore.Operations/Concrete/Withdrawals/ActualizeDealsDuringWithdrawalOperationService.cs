﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class ActualizeDealsDuringWithdrawalOperationService : IActualizeDealsDuringWithdrawalOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public ActualizeDealsDuringWithdrawalOperationService(
            IDealReadModel dealReadModel,
            IDealChangeStageAggregateService dealChangeStageAggregateService,
            IOperationScopeFactory scopeFactory, 
            ITracer tracer)
        {
            _dealReadModel = dealReadModel;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Actualize(IEnumerable<long> dealIds)
        {
            _tracer.Info("Starting actualizing deals during withdrawal process");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeDealsDuringWithdrawalIdentity>())
            {
                var dealInfos = _dealReadModel.GetInfoForWithdrawal(dealIds);

                _dealChangeStageAggregateService.ChangeStage(
                    dealInfos
                        .Where(i => i.Deal.DealStage != DealStage.Service && i.HasInactiveLocks)
                        .Select(i => new DealChangeStageDto
                            {
                                Deal = i.Deal,
                                NextStage = DealStage.Service
                            }));

                scope.Complete();
            }

            _tracer.Info("Finished actualizing deals during withdrawal process");
        }
    }
}