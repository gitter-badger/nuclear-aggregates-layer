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

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final
{
    public sealed class ReplicateToCRMMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private const int ReplicationTimeout = 60;
        private readonly IReplicationPersistenceService _replicationPersistenceService;
        private readonly ICommonLog _logger;

        public ReplicateToCRMMessageAggregatedProcessingResultHandler(
            IReplicationPersistenceService replicationPersistenceService,
            ICommonLog logger)
        {
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
                var msg = string.Format("Can't replication entity type {0} with ids: {1}", replicationFailedInfo.Item1, string.Join(";", replicationFailedInfo.Item2));
                _logger.ErrorEx(msg);
                
                throw new InvalidOperationException(msg);
            }

            return new HashSet<IMessageFlow>(new[] { FinalReplicate2MsCRMPerformedOperationsFlow.Instance });
        }

        private bool TryReplicate(IEnumerable<KeyValuePair<Type, List<long>>> replicationTargets, out Tuple<Type, IEnumerable<long>> replicateionFailed)
        {
            replicateionFailed = null;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var replicationTargetsBucket in replicationTargets)
                {
                    try
                    {
                        IEnumerable<long> failedReplication;
                        _replicationPersistenceService.ReplicateToMscrm(replicationTargetsBucket.Key,
                                                                        replicationTargetsBucket.Value,
                                                                        ReplicationTimeout,
                                                                        out failedReplication);
                        if (failedReplication != null && failedReplication.Any())
                        {
                            replicateionFailed = new Tuple<Type, IEnumerable<long>>(replicationTargetsBucket.Key, failedReplication);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormatEx(ex, "Can't replicate {0} entities of type {1}", replicationTargetsBucket.Value.Count, replicationTargetsBucket.Key);
                        throw;
                    }
                }

                transaction.Complete();
            }

            return true;
        }
    }
}