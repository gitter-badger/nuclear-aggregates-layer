using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public class ReplicateHotClientMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
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

        public bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return processingResults.All(m => m is ReplicateHotClientFinalProcessingResultsMessage);
        }

        public ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                _logger.WarnFormatEx("Replication to MsCRM disabled in config. Do nothing ...");
                return new HashSet<IMessageFlow>(new[] { FinalReplicateHotClientPerformedOperationsFlow.Instance });
            }

            var messagesToProcess = processingResults.Cast<ReplicateHotClientFinalProcessingResultsMessage>()
                                                     .Where(x => Equals(x.TargetFlow, FinalReplicateHotClientPerformedOperationsFlow.Instance))
                                                     .ToArray();
            
            // TODO {all, 17.07.2014}: возможно стоит вынести процесс создания task в operationservice, т.к. после отказа от MsCRM бизнесс процесс останется, просто действия будут заводиться в ERM
            // TODO {all, 17.07.2014}: учесть доработки по производительности final processing (не транзакционная обработка batch), fail одного не должен быть fail всего batch
            foreach (var message in messagesToProcess)
            {
                try
                {
                    if (message.RequestEntity.TaskId != null)
                    {
                        continue;
                    }

                    var taskId = CrmTaskUtils.ReplicateTask(_msCrmSettings.CreateDataContext(), message.Owner, message.HotClient, message.RegardingObject);
                    _bindCrmTaskToHotClientRequestAggregateService.BindWithCrmTask(message.RequestEntity, taskId);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't create hot client task for request with id = {0}", message.RequestEntity.Id);
                    throw;
                }
            }

            return new HashSet<IMessageFlow>(new[] { FinalReplicateHotClientPerformedOperationsFlow.Instance });
        }
    }
}