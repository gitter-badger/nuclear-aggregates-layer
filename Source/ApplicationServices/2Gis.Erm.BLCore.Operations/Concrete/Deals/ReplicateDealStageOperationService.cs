using System;

using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Deals
{
    public sealed class ReplicateDealStageOperationService : IReplicateDealStageOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;
        private readonly IUserImpersonationService _userImpersonationService;
        private readonly IActionLogger _actionLogger;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ReplicateDealStageOperationService(
            IDealReadModel dealReadModel,
            IDealChangeStageAggregateService dealChangeStageAggregateService, 
            IUserImpersonationService userImpersonationService,
            IActionLogger actionLogger,
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _dealReadModel = dealReadModel;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
            _userImpersonationService = userImpersonationService;
            _actionLogger = actionLogger;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Replicate(Guid dealReplicationCode, DealStage dealStage, string userDomainName)
        {
            _logger.DebugFormatEx("Deal replication. ReplicationCode: {0}, DealStage: {1}", dealReplicationCode, dealStage);

            if (dealStage == DealStage.OrderFormed
                || dealStage == DealStage.OrderApprovedForRelease
                || dealStage == DealStage.Service)
            {
                _logger.WarnFormatEx("Can't replicate deal - specified stage can't be applied using replication. ReplicationCode: {0}, DealStage: {1}", dealReplicationCode, dealStage);
                return;
            }

            using (var scope = _scopeFactory.CreateNonCoupled<ReplicateDealStageIdentity>())
            {
                // FIXME {all, 15.09.2013}: выглядит как дыра в безопасности - подумать, возможно нужно перед имперсонацией, всегда проверять функциональную привилегию, либо общую на имперсонацию, либо конкретную на данное действие (репликация deal) 
                if (!string.IsNullOrEmpty(userDomainName))
                {
                    // Меняем текущего пользователя, что бы в истории изменений стадии 
                    // сделки отображался корректный пользователь, а не тот, под которым хостится WCF сервис.
                    // C MS CRM имя пользователя приходит с доменом, в ERM же домен не учитывается.
                    _userImpersonationService.ImpersonateAsUser(IdentityUtils.GetAccount(userDomainName));
                }

                var deal = _dealReadModel.GetDeal(dealReplicationCode);
                if (deal.DealStage != (int)dealStage)
                {
                    var changes = _dealChangeStageAggregateService.ChangeStage(new[] { new DealChangeStageDto { Deal = deal, NextStage = dealStage } });
                    _actionLogger.LogChanges(changes);

                    scope.Updated<Deal>(deal.Id);
                }

                scope.Complete();
            }
        }
    }
}