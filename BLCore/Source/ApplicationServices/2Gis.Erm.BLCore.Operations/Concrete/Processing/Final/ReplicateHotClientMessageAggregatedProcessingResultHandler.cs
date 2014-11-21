using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public sealed class ReplicateHotClientMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IBindCrmTaskToHotClientRequestAggregateService _bindCrmTaskToHotClientRequestAggregateService;
        private readonly ICommonLog _logger;
        private readonly IMsCrmSettings _msCrmSettings;

        public ReplicateHotClientMessageAggregatedProcessingResultHandler(
            IMsCrmSettings msCrmSettings,
            IBindCrmTaskToHotClientRequestAggregateService bindCrmTaskToHotClientRequestAggregateService,
            ICommonLog logger)
        {
            _bindCrmTaskToHotClientRequestAggregateService = bindCrmTaskToHotClientRequestAggregateService;
            _logger = logger;
            _msCrmSettings = msCrmSettings;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                _logger.WarnFormatEx("Replication to MsCRM disabled in config. Do nothing ...");
                return processingResultBuckets.Select(pair => new KeyValuePair<Guid, MessageProcessingStageResult>(pair.Key, MessageProcessingStage.Handle.EmptyResult().AsSucceeded()));
            }

            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var hotClientInfos = new Dictionary<long, HotClientInfo>();

            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResults in processingResultBucket.Value)
                {
                    if (!Equals(processingResults.TargetFlow, FinalReplicateHotClientPerformedOperationsFlow.Instance))
                    {
                        continue;
                    }

                    var concreteProcessingResult = processingResults as ReplicateHotClientFinalProcessingResultsMessage;
                    if (concreteProcessingResult == null)
                    {
                        var messageProcessingStageResult = MessageProcessingStage.Handle
                                                                                 .EmptyResult()
                                                                                 .WithReport(string.Format("Unexpected processing result type {0} was achieved instead of {1}",
                                                                                                           processingResultBucket.Value.GetType().Name,
                                                                                                           typeof(ReplicateHotClientFinalProcessingResultsMessage).Name))
                                                                                 .AsFailed();
                        handlingResults.Add(processingResultBucket.Key, messageProcessingStageResult);

                        continue;
                    }

                    if (concreteProcessingResult.RequestEntity.TaskId != null)
                    {
                        _logger.WarnFormatEx(
                            "Hot client request with id {0} is processed already. Task id {1} and is not null. Skip further processing", 
                            concreteProcessingResult.RequestEntity.Id,
                            concreteProcessingResult.RequestEntity.TaskId);

                        handlingResults.Add(processingResultBucket.Key, MessageProcessingStage.Handle.EmptyResult().AsSucceeded());

                        continue;
                    }

                    HotClientInfo hotClientInfo;
                    if (!hotClientInfos.TryGetValue(concreteProcessingResult.RequestEntity.Id, out hotClientInfo))
                    {
                        hotClientInfo = new HotClientInfo { ReducedMessage = concreteProcessingResult };
                        hotClientInfos.Add(concreteProcessingResult.RequestEntity.Id, hotClientInfo);
                    }

                    hotClientInfo.OriginalMessageIds.Add(processingResultBucket.Key);
                }
            }

            foreach (var hotClientBucket in hotClientInfos)
            {
                var hotClientInfo = hotClientBucket.Value.ReducedMessage;

                MessageProcessingStageResult messageProcessingStageResult;

                try
                {
                    // TODO {all, 17.07.2014}: возможно стоит вынести процесс создания task в operationservice, т.к. после отказа от MsCRM бизнесс процесс останется, просто действия будут заводиться в ERM
                    var taskId = CrmTaskUtils.ReplicateTask(
                                    _msCrmSettings.CreateDataContext(), 
                                    hotClientInfo.Owner, 
                                    hotClientInfo.HotClient, 
                                    hotClientInfo.RegardingObject);

                    _bindCrmTaskToHotClientRequestAggregateService.BindWithCrmTask(hotClientInfo.RequestEntity, taskId);

                    messageProcessingStageResult = MessageProcessingStage.Handle
                                                                         .EmptyResult()
                                                                         .AsSucceeded();
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Can't create hot client task for request with id = {0}", hotClientInfo.RequestEntity.Id);
                    _logger.ErrorFormatEx(ex, msg);
                    
                    messageProcessingStageResult = MessageProcessingStage.Handle
                                                                         .EmptyResult()
                                                                         .WithExceptions(ex)
                                                                         .WithReport(msg)
                                                                         .AsFailed();
                }

                foreach (var originalMessageId in hotClientBucket.Value.OriginalMessageIds)
                {
                    handlingResults.Add(originalMessageId, messageProcessingStageResult);
                }
            }

            return handlingResults;
        }

        private sealed class HotClientInfo
        {
            public HotClientInfo()
            {
                OriginalMessageIds = new HashSet<Guid>();
            }

            public ISet<Guid> OriginalMessageIds { get; private set; }
            public ReplicateHotClientFinalProcessingResultsMessage ReducedMessage { get; set; }
        }
    }
}