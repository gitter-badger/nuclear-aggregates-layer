using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM
{
    public sealed partial class ReplicateToCRMMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private const int MaxSlicingAttempts = 4;
        private readonly IReplicationPersistenceService _replicationPersistenceService;
        private readonly IAsyncMsCRMReplicationSettings _asyncMsCRMReplicationSettings;
        private readonly ITracer _tracer;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;
        private readonly SlicerSettings _baseSlicerSettings;
        private readonly int _maxEffectiveDivisor;

        public ReplicateToCRMMessageAggregatedProcessingResultHandler(
            IAsyncMsCRMReplicationSettings asyncMsCRMReplicationSettings,
            IReplicationPersistenceService replicationPersistenceService,
            ITracer tracer,
            IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider)
        {
            _asyncMsCRMReplicationSettings = asyncMsCRMReplicationSettings;
            _tracer = tracer;
            _msCrmReplicationMetadataProvider = msCrmReplicationMetadataProvider;
            _replicationPersistenceService = replicationPersistenceService;


            _baseSlicerSettings = SlicerSettings.Default;
            _maxEffectiveDivisor = _baseSlicerSettings.Ratio ^ MaxSlicingAttempts;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var replicationTargets = new Dictionary<Type, List<Tuple<Guid, long>>>();

            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResults in processingResultBucket.Value)
                {
                    if (!Equals(processingResults.TargetFlow, FinalReplicate2MsCRMPerformedOperationsFlow.Instance))
                    {
                        continue;
                    }

                    var concreteProcessingResult = processingResults as ReplicateToCRMFinalProcessingResultsMessage;
                    if (concreteProcessingResult == null)
                    {
                            var messageProcessingResult = MessageProcessingStage.Handle
                                                                                .EmptyResult()
                                                                                .WithReport(string.Format("Unexpected processing result type {0} was achieved instead of {1}",
                                                                                                          processingResultBucket.Value.GetType().Name,
                                                                                                          typeof(ReplicateToCRMFinalProcessingResultsMessage).Name))
                                                                                .AsFailed();

                            handlingResults.Add(processingResultBucket.Key, messageProcessingResult);

                            continue;
                    }

                    List<Tuple<Guid, long>> replicationTargetsContainer;
                    if (!replicationTargets.TryGetValue(concreteProcessingResult.EntityType, out replicationTargetsContainer))
                    {
                        replicationTargetsContainer = new List<Tuple<Guid, long>>();
                        replicationTargets.Add(concreteProcessingResult.EntityType, replicationTargetsContainer);
                    }

                    replicationTargetsContainer.AddRange(concreteProcessingResult.Ids.Select(id => new Tuple<Guid, long>(processingResultBucket.Key, id)));
                }
            }

            foreach (var replicationType in _msCrmReplicationMetadataProvider.GetReplicationTypeSequence())
            {
                List<Tuple<Guid, long>> replicationBucket;
                if (!replicationTargets.TryGetValue(replicationType, out replicationBucket))
                {
                    continue;
                }

                var slicerSettings = EvaluateAdaptiveSlicerSettings(replicationBucket.Count);
                var replicationBucketSlicer = new Slicer<Tuple<Guid, long>>(slicerSettings, replicationBucket);

                IReadOnlyCollection<Tuple<Guid, long>> slicedReplicationBucket;
                while (replicationBucketSlicer.TryGetRange(out slicedReplicationBucket))
                {
                    IReadOnlyCollection<long> replicationFailed;
                    if (TryReplicate(replicationType, slicedReplicationBucket, out replicationFailed))
                    {
                        foreach (var replicated in slicedReplicationBucket)
                        {
                            handlingResults.Add(replicated.Item1, MessageProcessingStage.Handle.EmptyResult().AsSucceeded());
                        }

                        replicationBucketSlicer.Shift();
                        continue;
                    }

                    if (!replicationBucketSlicer.TrySliceSmaller())
                    {
                        foreach (var replicated in slicedReplicationBucket)
                        {
                            handlingResults.Add(replicated.Item1, MessageProcessingStage.Handle.EmptyResult().WithReport("Can't replicate").AsFailed());
                        }

                        replicationBucketSlicer.Shift();
                    }
                }
            }

            return handlingResults;
        }

        private SlicerSettings EvaluateAdaptiveSlicerSettings(int currentReplicationQueueLength)
        {
            return new SlicerSettings
                       {
                           MaxRangeLength = _baseSlicerSettings.MaxRangeLength,
                           MinRangeLength = Math.Max(1, currentReplicationQueueLength / _maxEffectiveDivisor),
                           Ratio = _baseSlicerSettings.Ratio
                       };
        }

        private bool TryReplicate(Type replicationType, IReadOnlyCollection<Tuple<Guid, long>> replicationTargets, out IReadOnlyCollection<long> replicationFailed)
        {
            replicationFailed = null;
            var replicationEntities = replicationTargets.Select(x => x.Item2).Distinct().ToArray();
            
            try
            {
                _replicationPersistenceService.ReplicateToMsCrm(replicationType,
                                                                replicationEntities,
                                                                TimeSpan.FromSeconds(_asyncMsCRMReplicationSettings.ReplicationTimeoutSec),
                                                                out replicationFailed);

                return replicationFailed == null || !replicationFailed.Any();
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Can't replicate {0} entities of type {1}", replicationTargets.Count, replicationType);
                return false;
            }
        }
    }
}