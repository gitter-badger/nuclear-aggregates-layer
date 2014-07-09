using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors
{
    public abstract partial class MessageFlowProcessorBase<TMessageFlow, TMessageFlowProcessorSettings, TMessageReceiverSettings> : ISyncMessageFlowProcessor, IAsyncMessageFlowProcessor
        where TMessageFlow : class, IMessageFlow, new()
        where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings
        where TMessageReceiverSettings : class, IMessageReceiverSettings
    {
        protected readonly TMessageFlow SourceMessageFlow = new TMessageFlow();
        protected readonly TMessageFlowProcessorSettings ProcessorSettings;

        private readonly IMessageReceiverFactory _messageReceiverFactory;
        private readonly IMessageProcessingTopology _processingTopology;
        private readonly ICommonLog _logger;

        private readonly CancellationTokenSource _workerCTS;
        private readonly Task _workerTask;

        protected MessageFlowProcessorBase(
            TMessageFlowProcessorSettings processorSettings,
            IMessageReceiverFactory messageReceiverFactory,
            IMessageProcessingTopology processingTopology,
            ICommonLog logger)
        {
            ProcessorSettings = processorSettings;
            _messageReceiverFactory = messageReceiverFactory;
            _processingTopology = processingTopology;
            _logger = logger;

            _workerCTS = new CancellationTokenSource();
            _workerTask = new Task(AsyncWorker, _workerCTS.Token, TaskCreationOptions.LongRunning);
        }

        protected ICommonLog Logger
        {
            get { return _logger; }
        }

        void ISyncMessageFlowProcessor.Process()
        {
            try
            {
                Process();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormatEx(ex, "Sync processing for message flow " + SourceMessageFlow + " failed");
                throw;
            }
        }

        void IAsyncMessageFlowProcessor.Start()
        {
            _workerTask.Start();
        }

        void IAsyncMessageFlowProcessor.Stop()
        {
            if (!_workerTask.IsCompleted && !_workerTask.IsCanceled)
            {
                _workerCTS.Cancel();

                try
                {
                    _workerTask.Wait();
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, "Can't stop async processor for flow " + SourceMessageFlow);
                    throw;
                }
            }
        }

        void IAsyncMessageFlowProcessor.Wait()
        {
            try
            {
                _workerTask.Wait();
            }
            catch (Exception ex)
            {
                Logger.ErrorEx(ex, "Waiting for async processing results finished unexpectedly" + SourceMessageFlow);
                throw;
            }
        }

        protected int Process()
        {
            var messageReceiverSettings = ResolveReceiverSettings();

            IMessageReceiver messageReceiver;

            try
            {
                messageReceiver = _messageReceiverFactory.Create<TMessageFlow, TMessageReceiverSettings>(messageReceiverSettings);
            }
            catch (Exception ex)
            {
                Logger.ErrorEx(ex, "Can't create appropriate message receiver for specified flow " + SourceMessageFlow);
                throw;
            }

            var stopwatch = new Stopwatch();
            IMessage[] flowMessages = null;

            try
            {
                stopwatch.Start();

                Logger.DebugEx("Starting processing message flow. Receiving messages");
                try
                {
                    flowMessages = messageReceiver.Peek()
                                                  .ToArray();
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, "Can't receive messages from flow " + SourceMessageFlow);
                    throw;
                }

                Logger.DebugFormatEx("Starting processing message flow. Acquired messages batch size: {0}. Source message flow: {1}", flowMessages.Length, SourceMessageFlow);

                TopologyProcessingResults topologyProcessingResults;
                try
                {
                    var processingResult = _processingTopology.ProcessAsync(flowMessages);
                    processingResult.Wait();
                    topologyProcessingResults = processingResult.Result;
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, "Can't process messages from flow " + SourceMessageFlow + ". Trying to report failed messages");
                    try
                    {
                        messageReceiver.Complete(Enumerable.Empty<IMessage>(), flowMessages);
                    }
                    catch (Exception nex)
                    {
                        Logger.ErrorEx(nex, "Can't report failed messages");
                        throw;
                    }

                    throw;
                }

                var processingSummary = string.Format(
                    "Total pass through messages count: {0}. Succeeded: {1}. Failed: {2}",
                    topologyProcessingResults.Passed.Length,
                    topologyProcessingResults.Succeeded.Length,
                    topologyProcessingResults.Failed.Length);
                Logger.DebugEx(processingSummary);
                if (topologyProcessingResults.Failed.Any())
                {
                    Logger.ErrorEx("Messages processing has failed elements. " + processingSummary);
                }

                messageReceiver.Complete(topologyProcessingResults.Succeeded, topologyProcessingResults.Failed);

                return flowMessages.Length;
            }
            catch (Exception ex)
            {
                Logger.ErrorEx(ex, "Can't process messages from specified flow " + SourceMessageFlow);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                var batchProcessingTime = stopwatch.ElapsedMilliseconds / 1000.0;
                var rateMsg = flowMessages == null || flowMessages.Length == 0
                                  ? "not measured 0 messages was provided by receiver"
                                  : (ProcessorSettings.MessageBatchSize / batchProcessingTime).ToString();
                _logger.DebugEx("Processing rate msg/sec: " + rateMsg);

                var disposableMessageReceiver = messageReceiver as IDisposable; 
                if (disposableMessageReceiver != null)
                {
                    disposableMessageReceiver.Dispose();
                }
            }
        }

        protected abstract TMessageReceiverSettings ResolveReceiverSettings();

        private void AsyncWorker()
        {
            const int BaseDelayMs = 500;
            const int DelayIncrementMs = 1000;
            int currentDelay = BaseDelayMs;

            while (!_workerCTS.Token.IsCancellationRequested)
            {
                int processedCount = 0;
                try
                {
                    processedCount = Process();
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormatEx(ex, "Async processing for message flow " + SourceMessageFlow + " failed. Processing will be continued after some delay");
                }

                if (processedCount > 0)
                {
                    currentDelay = BaseDelayMs;
                }
                else
                {
                    currentDelay += DelayIncrementMs;
                }

                Thread.Sleep(currentDelay);
            }
        }
    }
}