using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public sealed class HotClientMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IProcessHotClientRequestOperationService _processHotClientRequestOperationService;
        private readonly ITracer _tracer;

        public HotClientMessageAggregatedProcessingResultHandler(
            IProcessHotClientRequestOperationService processHotClientRequestOperationService,
            ITracer tracer)
        {
            _processHotClientRequestOperationService = processHotClientRequestOperationService;
            _tracer = tracer;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var hotClientInfos = new Dictionary<long, HotClientInfo>();

            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResults in processingResultBucket.Value)
                {
                    if (!Equals(processingResults.TargetFlow, FinalProcessHotClientPerformedOperationsFlow.Instance))
                    {
                        continue;
                    }

                    var concreteProcessingResult = processingResults as HotClientFinalProcessingResultsMessage;
                    if (concreteProcessingResult == null)
                    {
                        var messageProcessingStageResult = MessageProcessingStage.Handle
                                                                                 .EmptyResult()
                                                                                 .WithReport(string.Format("Unexpected processing result type {0} was achieved instead of {1}",
                                                                                                           processingResultBucket.Value.GetType().Name,
                                                                                                           typeof(HotClientFinalProcessingResultsMessage).Name))
                                                                                 .AsFailed();
                        handlingResults.Add(processingResultBucket.Key, messageProcessingStageResult);

                        continue;
                    }

                    if (concreteProcessingResult.HotClientRequest.HasAssignedTask)
                    {
                        _tracer.WarnFormat(
                            "Hot client request with id {0} has been already processed and a task has been assigned. Skip the request processing.",
                            concreteProcessingResult.HotClientRequest.Id);

                        handlingResults.Add(processingResultBucket.Key, MessageProcessingStage.Handle.EmptyResult().AsSucceeded());

                        continue;
                    }

                    HotClientInfo hotClientInfo;
                    if (!hotClientInfos.TryGetValue(concreteProcessingResult.HotClientRequest.Id, out hotClientInfo))
                    {
                        hotClientInfo = new HotClientInfo { ReducedMessage = concreteProcessingResult };
                        hotClientInfos.Add(concreteProcessingResult.HotClientRequest.Id, hotClientInfo);
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
                    _processHotClientRequestOperationService.CreateHotClientTask(
                        hotClientInfo.HotClientRequest,
                        hotClientInfo.OwnerId,
                        hotClientInfo.RegardingObject);

                    messageProcessingStageResult = MessageProcessingStage.Handle
                                                                         .EmptyResult()
                                                                         .AsSucceeded();
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Can't create hot client task for request with id = {0}", hotClientInfo.HotClientRequest.Id);
                    _tracer.ErrorFormat(ex, msg);
                    
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
            public HotClientFinalProcessingResultsMessage ReducedMessage { get; set; }
        }
    }
}