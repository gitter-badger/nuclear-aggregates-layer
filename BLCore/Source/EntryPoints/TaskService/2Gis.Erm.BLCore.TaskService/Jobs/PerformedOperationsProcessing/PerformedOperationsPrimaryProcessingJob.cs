using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Jobs;

using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing
{
    [DisallowConcurrentExecution]
    public sealed class PerformedOperationsPrimaryProcessingJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IMessageFlowRegistry _messageFlowRegistry;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;

        private readonly object _performedOperationsProcessorSync = new object();
        private IAsyncMessageFlowProcessor _performedOperationsProcessor;

        public PerformedOperationsPrimaryProcessingJob(
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
        public string Flow { get; set; }
        public int? TimeSafetyOffsetHours { get; set; }

        private IAsyncMessageFlowProcessor MessageFlowProcessor
        {
            get
            {
                lock (_performedOperationsProcessorSync)
                {
                    return _performedOperationsProcessor;
                }
            }
            set
            {
                lock (_performedOperationsProcessorSync)
                {
                    _performedOperationsProcessor = value;
                }
            }
        }

        public void Interrupt()
        {
            var flowProcessor = MessageFlowProcessor;
            if (flowProcessor != null)
            {
                flowProcessor.Stop();
            }
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                Tracer.InfoFormat("Integration disabled in settings. Job stops immediately");
                return;
            }

            IMessageFlow messageFlow;
            if (!_messageFlowRegistry.TryResolve(Flow, out messageFlow))
            {
                string msg = "Unsupported flow specified for processing: " + Flow;
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            Tracer.Debug("Launching message flow processing. Target message flow: " + messageFlow);

            try
            {
                var processorSettings = new PerformedOperationsPrimaryFlowProcessorSettings
                    {
                        MessageBatchSize = BatchSize,
                        AppropriatedStages = new[] { MessageProcessingStage.Transforming, MessageProcessingStage.Processing, MessageProcessingStage.Handle },
                        IgnoreErrorsOnStage = new MessageProcessingStage[0],
                        TimeSafetyOffsetHours = TimeSafetyOffsetHours
                    };

                MessageFlowProcessor = _messageFlowProcessorFactory.CreateAsync<IPerformedOperationsFlowProcessorSettings>(messageFlow, processorSettings);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for  specified flow " + messageFlow);
                throw;
            }

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlow);
                MessageFlowProcessor.Start();

                Tracer.Debug("Message flow processor started, waiting for finish ... Target message flow: " + messageFlow);
                MessageFlowProcessor.Wait();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlow);
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlow);
                throw;
            }
            finally
            {
                var flowProcessor = MessageFlowProcessor;
                if (flowProcessor != null)
                {
                    flowProcessor.Dispose();
                }
            }
        }
    }
}
