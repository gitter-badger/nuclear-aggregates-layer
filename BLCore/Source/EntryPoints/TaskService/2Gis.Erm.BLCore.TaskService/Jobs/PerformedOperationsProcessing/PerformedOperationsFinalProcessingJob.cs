using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Jobs;

using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing
{
    [DisallowConcurrentExecution]
    public class PerformedOperationsFinalProcessingJob : TaskServiceJobBase
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IMessageFlowRegistry _messageFlowRegistry;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;

        private IEnumerable<MessageProcessingStage> _ignoreErrorsOnStageSetting;

        public PerformedOperationsFinalProcessingJob(
            IIntegrationSettings integrationSettings,
            IMessageFlowRegistry messageFlowRegistry,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService, 
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _integrationSettings = integrationSettings;
            _messageFlowRegistry = messageFlowRegistry;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
        }

        public int BatchSize { get; set; }
        public string Flows { get; set; }
        public int? ReprocessingBatchSize { get; set; }
        public string IgnoreErrorsOnStages { get; set; }

        private IEnumerable<MessageProcessingStage> IgnoreErrorsOnStageSetting
        {
            get
            {
                if (string.IsNullOrEmpty(IgnoreErrorsOnStages))
                {
                    return Enumerable.Empty<MessageProcessingStage>();
                }

                if (_ignoreErrorsOnStageSetting != null)
                {
                    return _ignoreErrorsOnStageSetting;
                }
               
                var ignoreErrorsOnStage = new List<MessageProcessingStage>();
                foreach (var rawStage in IgnoreErrorsOnStages.Split(';'))
                {
                    MessageProcessingStage stage;
                    if (Enum.TryParse(rawStage, true, out stage))
                    {
                        ignoreErrorsOnStage.Add(stage);
                    }
                    else
                    {
                        var msg = string.Format("IgnoreErrorsOnStages setting for job with type {0} has invalid value: \"{1}\", value segment: {2}. " +
                                                "Please check settings",
                                                typeof(PerformedOperationsFinalProcessingJob),
                                                IgnoreErrorsOnStages,
                                                rawStage);
                        Tracer.Error(msg);

                        throw new InvalidOperationException(msg);
                    }
                }

                _ignoreErrorsOnStageSetting = ignoreErrorsOnStage;

                return _ignoreErrorsOnStageSetting;
            }
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                Tracer.InfoFormat("Integration disabled in settings. Job stops immediately");
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
                var msg = "Unsupported flow specified for processing: " + flowDescriptor;
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            ISyncMessageFlowProcessor messageFlowProcessor;

            Tracer.Debug("Launching message flow processing. Target message flow: " + messageFlow);

            try
            {
                messageFlowProcessor = _messageFlowProcessorFactory.CreateSync<IPerformedOperationsFinalFlowProcessorSettings>(
                   messageFlow,
                   new PerformedOperationsFinalFlowProcessorSettings
                   {
                       MessageBatchSize = BatchSize,
                       AppropriatedStages = new[] { MessageProcessingStage.Transforming, MessageProcessingStage.Processing, MessageProcessingStage.Handle },
                       IgnoreErrorsOnStage = IgnoreErrorsOnStageSetting,
                       ReprocessingBatchSize = ReprocessingBatchSize.HasValue ? ReprocessingBatchSize.Value : BatchSize
                   });
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for  specified flow " + messageFlow);
                throw;
            }

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlow);
                messageFlowProcessor.Process();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlow);
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlow);
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