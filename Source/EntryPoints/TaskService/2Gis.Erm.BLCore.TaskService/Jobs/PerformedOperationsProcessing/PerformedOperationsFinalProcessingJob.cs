using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing
{
    [DisallowConcurrentExecution]
    public sealed class PerformedOperationsFinalProcessingJob : TaskServiceJobBase
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IMessageFlowRegistry _messageFlowRegistry;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;

        public PerformedOperationsFinalProcessingJob(
            IIntegrationSettings integrationSettings,
            IMessageFlowRegistry messageFlowRegistry,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService, 
            ICommonLog logger)
            : base(signInService, userImpersonationService, logger)
        {
            _integrationSettings = integrationSettings;
            _messageFlowRegistry = messageFlowRegistry;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
        }

        public int BatchSize { get; set; }
        public string Flows { get; set; }
        public bool RecoveryMode { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                Logger.InfoFormatEx("Integration disabled in settings. Job stops immediately");
                return;
            }

            var targetFlows = Flows.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var targetFlow in targetFlows)
            {
                ProcessFlow(targetFlow);
            }
        }

        private void ProcessFlow(string flowDescriptor)
        {
            IMessageFlow messageFlow;
            if (!_messageFlowRegistry.TryResolve(flowDescriptor, out messageFlow))
            {
                string msg = "Unsupported flow specified for processing: " + flowDescriptor;
                Logger.FatalEx(msg);
                throw new InvalidOperationException(msg);
            }

            ISyncMessageFlowProcessor messageFlowProcessor;

            Logger.DebugEx("Launching message flow processing. Target message flow: " + messageFlow);

            try
            {
                messageFlowProcessor = _messageFlowProcessorFactory.CreateSync<IPerformedOperationsFinalFlowProcessorSettings>(
                   messageFlow,
                   new PerformedOperationsFinalFlowProcessorSettings
                   {
                       MessageBatchSize = BatchSize,
                       AppropriatedStages = new[] { MessageProcessingStage.Transforming, MessageProcessingStage.Processing, MessageProcessingStage.Handle },
                       IgnoreErrorsOnStage = new MessageProcessingStage[0],
                       IsRecoveryMode = RecoveryMode
                   });
            }
            catch (Exception ex)
            {
                Logger.ErrorEx(ex, "Can't create processor for  specified flow " + messageFlow);
                throw;
            }

            try
            {
                Logger.DebugEx("Message flow processor starting. Target message flow: " + messageFlow);
                messageFlowProcessor.Process();
                Logger.DebugEx("Message flow processor finished. Target message flow: " + messageFlow);
            }
            catch (Exception ex)
            {
                Logger.FatalEx(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlow);
                throw;
            }
            finally
            {
                if (messageFlowProcessor != null)
                {
                    messageFlowProcessor.Dispose();
                }
            }
        }
    }
}