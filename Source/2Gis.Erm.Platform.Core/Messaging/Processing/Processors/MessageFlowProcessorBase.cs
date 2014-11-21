﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        private readonly CancellationTokenSource _workerCancellationTokenSource;
        private readonly Task _workerTask;
        private AutoResetEvent _asyncWorkerSignal;

        protected MessageFlowProcessorBase(TMessageFlowProcessorSettings processorSettings,
            IMessageReceiverFactory messageReceiverFactory,
            IMessageProcessingTopology processingTopology,
            ICommonLog logger)
        {
            ProcessorSettings = processorSettings;
            _messageReceiverFactory = messageReceiverFactory;
            _processingTopology = processingTopology;
            _logger = logger;

            _workerCancellationTokenSource = new CancellationTokenSource();
            _workerTask = new Task(AsyncWorker, _workerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
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
                Logger.ErrorFormatEx(ex, "Failed sync processing for message flow " + SourceMessageFlow);
                throw;
            }
        }

        void IAsyncMessageFlowProcessor.Start()
        {
            _asyncWorkerSignal = new AutoResetEvent(false);
            _workerTask.Start();
        }

        void IAsyncMessageFlowProcessor.Stop()
        {
            if (!_workerTask.IsCompleted && !_workerTask.IsCanceled)
            {
                _workerCancellationTokenSource.Cancel();

                var asyncWorkerSignal = _asyncWorkerSignal;
                if (asyncWorkerSignal != null)
                {
                    asyncWorkerSignal.Set();
                }

                try
                {
                    _workerTask.Wait();
                    _asyncWorkerSignal.Close();
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, "Can't stop async processor for flow " + SourceMessageFlow);
                    throw;
                }
                finally
                {
                    if (asyncWorkerSignal != null)
                    {
                        asyncWorkerSignal.Close();
                    }
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

        protected abstract TMessageReceiverSettings ResolveReceiverSettings();

        private ProcessingResult Process()
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
            IReadOnlyList<IMessage> flowMessages = null;

            try
            {
                stopwatch.Start();

                Logger.DebugFormatEx("Starting processing message flow {0}. Receiving messages", SourceMessageFlow);

                try
                {
                    flowMessages = messageReceiver.Peek();
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, "Can't receive messages from flow " + SourceMessageFlow);
                    throw;
                }

                if (!flowMessages.Any())
                {
                    Logger.DebugFormatEx("Further flow {0} processing skipped, because no message received, possible transport is empty", SourceMessageFlow);
                    return new ProcessingResult
                               {
                                   ActuallyProcessedMessageCount = 0, 
                                   RequestedMessageBatchSize = messageReceiverSettings.BatchSize
                               };
                }

                Logger.DebugFormatEx("Starting processing message flow. Acquired messages batch size: {0}. Source message flow: {1}", flowMessages.Count, SourceMessageFlow);

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
                        Logger.ErrorEx(nex, "Can't report failed messages, flow details: " + SourceMessageFlow);
                        throw;
                    }

                    throw;
                }

                var processingSummary = string.Format("Total original messages count: {0}. Succeeded: {1}. Failed: {2}",
                                                      flowMessages.Count,
                                                      topologyProcessingResults.Succeeded.Count,
                                                      topologyProcessingResults.Failed.Count);
                Logger.DebugEx(processingSummary);
                if (topologyProcessingResults.Failed.Any())
                {
                    Logger.ErrorFormatEx("Messages form flow {0} after processing has failed elements. {1}", SourceMessageFlow, processingSummary);
                }

                /* проверка скорости работы транспорта при исключении топологии 
                topologyProcessingResults = new TopologyProcessingResults()
                    {
                        Failed = new IMessage[0],
                        Passed = flowMessages,
                        Succeeded = flowMessages
                    };*/


                messageReceiver.Complete(topologyProcessingResults.Succeeded, topologyProcessingResults.Failed);

                return new ProcessingResult
                               {
                                   ActuallyProcessedMessageCount = flowMessages.Count, 
                                   RequestedMessageBatchSize = messageReceiverSettings.BatchSize
                               };
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
                var rateMsg = flowMessages == null || flowMessages.Count == 0
                                  ? "not measured 0 messages was provided by receiver"
                                  : (flowMessages.Count / batchProcessingTime).ToString();
                _logger.DebugFormatEx("Processing flow {0} rate msg/sec: {1}", SourceMessageFlow, rateMsg);

                var disposableMessageReceiver = messageReceiver as IDisposable; 
                if (disposableMessageReceiver != null)
                {
                    disposableMessageReceiver.Dispose();
                }
            }
        }

        private void AsyncWorker()
        {
            const int BaseDelayMs = 0;
            const int DelayIncrementMs = 2000;
            const int DelayAfterFailure = 45000;
            const int MaxDelayMs = 150000;
            const decimal SufficientBatchUtilizationMinPercentage = 10M;

            int currentDelay = BaseDelayMs;
            while (!_workerCancellationTokenSource.Token.IsCancellationRequested)
            {
                ProcessingResult processingResult = null;

                try
                {
                    processingResult = Process();
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormatEx(ex, "Failed async processing for message flow " + SourceMessageFlow + ". Processing will be continued after some delay");
                }

                if (processingResult == null)
                {
                    currentDelay = DelayAfterFailure;
                    Logger.InfoFormatEx("Processing flow {0}. Restoration delay after previous failure was applied ms: {1}", SourceMessageFlow, currentDelay);
                }
                else
                {
                    decimal effectiveBatchUtilizationPercentage = processingResult.ActuallyProcessedMessageCount * 100M / processingResult.RequestedMessageBatchSize;
                    if (effectiveBatchUtilizationPercentage > SufficientBatchUtilizationMinPercentage)
                    {
                        currentDelay = BaseDelayMs;
                        Logger.DebugFormatEx(
                            "Processing flow {0}. {1} messages was handled during the last cycle. Message batch utilization: {2:F1}%. Delay has base value ms: {3}",
                            SourceMessageFlow,
                            processingResult.ActuallyProcessedMessageCount,
                            effectiveBatchUtilizationPercentage,
                            currentDelay);
                    }
                    else
                    {
                        currentDelay = Math.Min(currentDelay + DelayIncrementMs, MaxDelayMs);
                        Logger.DebugFormatEx(
                            "Processing flow {0}. Message batch size {1} utilization is {2:F1}% and lower than min sufficient value {3:F1}%. Delay was incremented and has value ms: {4}",
                            processingResult.RequestedMessageBatchSize,
                            effectiveBatchUtilizationPercentage,
                            SufficientBatchUtilizationMinPercentage,
                            SourceMessageFlow,
                            currentDelay);
                    }
                }

                _asyncWorkerSignal.WaitOne(currentDelay);
            }
        }

        private sealed class ProcessingResult
        {
            public int ActuallyProcessedMessageCount { get; set; }
            public int RequestedMessageBatchSize { get; set; }
        }
    }
}