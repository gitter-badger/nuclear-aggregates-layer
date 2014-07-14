using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM
{
    public sealed class ReplicateToCRMMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IReplicationPersistenceService _replicationPersistenceService;
        private readonly IAsyncMsCRMReplicationSettings _asyncMsCRMReplicationSettings;
        private readonly ICommonLog _logger;

        private readonly Type[] _replicationTypeSequence =
            {
                typeof(Territory), 
                typeof(Firm), 
                typeof(FirmAddress)
            };

        public ReplicateToCRMMessageAggregatedProcessingResultHandler(
            IAsyncMsCRMReplicationSettings asyncMsCRMReplicationSettings,
            IReplicationPersistenceService replicationPersistenceService,
            ICommonLog logger)
        {
            _asyncMsCRMReplicationSettings = asyncMsCRMReplicationSettings;
            _logger = logger;
            _replicationPersistenceService = replicationPersistenceService;
        }

        public bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return processingResults.All(m => m is ReplicateToCRMFinalProcessingResultsMessage);
        }

        public ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            var replicationTargets = new Dictionary<Type, List<long>>();
            
            foreach (var processingResult in processingResults)
            {
                if (!Equals(processingResult.TargetFlow, FinalReplicate2MsCRMPerformedOperationsFlow.Instance))
                {
                    continue;
                }

                var concreteProcessingResult = processingResult as ReplicateToCRMFinalProcessingResultsMessage;
                if (concreteProcessingResult == null)
                {
                    throw new InvalidOperationException(string.Format("Unexpected processing result type {0} was achieved instead of {1}",
                                                                      processingResults.GetType().Name,
                                                                      typeof(ReplicateToCRMFinalProcessingResultsMessage).Name));
                }

                List<long> replicationTargetsContainer;
                if (!replicationTargets.TryGetValue(concreteProcessingResult.EntityType, out replicationTargetsContainer))
                {
                    replicationTargetsContainer = new List<long>();
                    replicationTargets.Add(concreteProcessingResult.EntityType, replicationTargetsContainer);
                }

                replicationTargetsContainer.AddRange(concreteProcessingResult.Ids);
            }

            Tuple<Type, IEnumerable<long>> replicationFailedInfo;
            if (!TryReplicate(replicationTargets, out replicationFailedInfo))
            {
                var msg = string.Format("Can't replicate entity type {0} with ids: {1}", replicationFailedInfo.Item1, string.Join(";", replicationFailedInfo.Item2));
                _logger.ErrorEx(msg);
                
                throw new InvalidOperationException(msg);
            }

            return new HashSet<IMessageFlow>(new[] { FinalReplicate2MsCRMPerformedOperationsFlow.Instance });
        }

        private bool TryReplicate(IReadOnlyDictionary<Type, List<long>> replicationTargets, out Tuple<Type, IEnumerable<long>> replicateionFailed)
        {
            replicateionFailed = null;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var replicationType in _replicationTypeSequence)
                {
                    List<long> entitiesList;
                    if (!replicationTargets.TryGetValue(replicationType, out entitiesList))
                    {
                        continue;
                    }

                    try
                    {
                        IEnumerable<long> failedReplication;
                        _replicationPersistenceService.ReplicateToMscrm(replicationType,
                                                                        entitiesList,
                                                                        _asyncMsCRMReplicationSettings.ReplicationTimeoutSec,
                                                                        out failedReplication);
                        if (failedReplication != null && failedReplication.Any())
                        {
                            replicateionFailed = new Tuple<Type, IEnumerable<long>>(replicationType, failedReplication);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormatEx(ex, "Can't replicate {0} entities of type {1}", entitiesList.Count, replicationType);
                        throw;
                    }
                }

                transaction.Complete();
            }

            return true;
        }
    }
}