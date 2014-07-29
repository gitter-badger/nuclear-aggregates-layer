﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationLogger : IOperationLogger
    {
        private readonly IReadOnlyCollection<IOperationLoggingStrategy> _loggingStrategies;
        private readonly ICommonLog _logger;

        public OperationLogger(
            // ReSharper disable ParameterTypeCanBeEnumerable.Local // unity registrations 1..*
            IOperationLoggingStrategy[] loggingStrategies,
            // ReSharper restore ParameterTypeCanBeEnumerable.Local
            ICommonLog logger)
        {
            _loggingStrategies = loggingStrategies;
            _logger = logger;
        }

        public void Log(TrackedUseCase useCase)
        {
            //SequentialLogging(useCase);
            ParallelLogging(useCase);
        }

        // COMMENT {all, 29.07.2014}: пока оставлена реализация логирования строго последовательная, до ввода service bus в промышленную эксплуатацию, на случай, если с parallel возникнут, какие-то трудности
        private void SequentialLogging(TrackedUseCase useCase)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var loggingSessions = new List<Tuple<IOperationLoggingStrategy, LoggingSession>>();

            bool isUseCaseLoggedSuccessfully = true;

            foreach (var strategy in _loggingStrategies)
            {
                var loggingSession = strategy.Begin();
                loggingSessions.Add(new Tuple<IOperationLoggingStrategy, LoggingSession>(strategy, loggingSession));

                var strategyStopwatch = new Stopwatch();
                strategyStopwatch.Start();
                
                try
                {
                    string report;
                    if (!strategy.TryLogUseCase(useCase, out report))
                    {
                        _logger.ErrorFormatEx("Can't log use case {0} through strategy {1}. Details: {2}", useCase, strategy.GetType(), report);
                        isUseCaseLoggedSuccessfully = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't log use case {0} through strategy {1}", useCase, strategy.GetType());
                    isUseCaseLoggedSuccessfully = false;
                }
                finally
                {
                    strategyStopwatch.Stop();
                    _logger.DebugFormatEx(
                        "Logging strategy {0} executed. It takes {1} sec. Usecase details: {2}",
                        strategy.GetType().Name,
                        strategyStopwatch.Elapsed.TotalSeconds,
                        useCase);
                }
            }

            for (int i = loggingSessions.Count - 1; i >= 0; i--)
            {
                var session = loggingSessions[i];
                if (isUseCaseLoggedSuccessfully)
                {
                    session.Item1.Complete(session.Item2);
                }

                session.Item1.Close(session.Item2);
            }

            stopwatch.Stop();
            _logger.DebugFormatEx("Sequential logging of use case {0} takes {1} sec", useCase, stopwatch.Elapsed.TotalSeconds);

            if (!isUseCaseLoggedSuccessfully)
            {
                var msg = "Can't log use case " + useCase + " logging aborted";
                _logger.ErrorEx(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private void ParallelLogging(TrackedUseCase useCase)
        {
            var stopwatch = new Stopwatch();
            var finishSignal = new ManualResetEvent(false);
            var logWorkerFinishMode = new LogWorkerFinishMode { CompleteLogging = true };

            WaitHandle[] loggingExecutedSignals;
            IReadOnlyCollection<LogWorkerResult> workerResults;

            stopwatch.Start();

            var workers = ResolveLogWorkers(useCase, finishSignal, logWorkerFinishMode, out loggingExecutedSignals, out workerResults);

            foreach (var worker in workers)
            {
                worker.Start();
            }

            WaitHandle.WaitAll(loggingExecutedSignals);

            foreach (var result in workerResults)
            {
                if (!result.Succeeded)
                {
                    _logger.ErrorEx("One of the workers failed: " + result.Report);
                    logWorkerFinishMode.CompleteLogging = false;
                }
            }

            finishSignal.Set();

            try
            {
                Task.WaitAll(workers);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't log usecase {0}. One of the logging workers failed", useCase);
                logWorkerFinishMode.CompleteLogging = false;
            }

            finishSignal.Close();
            foreach (var signal in loggingExecutedSignals)
            {
                signal.Close();
            }

            stopwatch.Stop();
            _logger.DebugFormatEx("Parallel logging of use case {0} takes {1} sec", useCase, stopwatch.Elapsed.TotalSeconds);

            if (!logWorkerFinishMode.CompleteLogging)
            {
                var msg = "Can't log use case " + useCase + " logging aborted";
                _logger.ErrorEx(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private Task[] ResolveLogWorkers(
            TrackedUseCase useCase,
            ManualResetEvent finishSignal,
            LogWorkerFinishMode logWorkerFinishMode,
            out WaitHandle[] loggingExecutedSignals,
            out IReadOnlyCollection<LogWorkerResult> workersResults)
        {
            var workers = new List<Task>();
            var signals = new List<WaitHandle>();
            var results = new List<LogWorkerResult>();

            foreach (var strategy in _loggingStrategies)
            {
                var executedSignal = new AutoResetEvent(false);
                var result = new LogWorkerResult();
                var context = new LogWorkerContext
                    {
                        UseCase = useCase,
                        LoggingStrategy = strategy,
                        ExecutedSignal = executedSignal,
                        FinishSignal = finishSignal,
                        FinishMode = logWorkerFinishMode,
                        Result = result
                    };

                workers.Add(new Task(LogWorker, context));
                signals.Add(executedSignal);
                results.Add(result);
            }

            loggingExecutedSignals = signals.ToArray();
            workersResults = results;
            return workers.ToArray();
        }

        private void LogWorker(object context)
        {
            var concreteContext = (LogWorkerContext)context;

            var loggingSession = concreteContext.LoggingStrategy.Begin();

            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    string report;
                    var isSuccessfullyLogged = concreteContext.LoggingStrategy.TryLogUseCase(concreteContext.UseCase, out report);

                    concreteContext.Result.Report = report;
                    concreteContext.Result.Succeeded = isSuccessfullyLogged;
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Can't log use case {0} properly through logging strategy {1}",
                                            concreteContext.UseCase,
                                            concreteContext.LoggingStrategy.GetType());
                    _logger.ErrorEx(ex, msg);

                    concreteContext.Result.Report = msg;
                    concreteContext.Result.Succeeded = false;
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.DebugFormatEx(
                        "Logging strategy {0} executed. It takes {1} sec. Usecase details: {2}",
                        concreteContext.LoggingStrategy.GetType().Name,
                        stopwatch.Elapsed.TotalSeconds,
                        concreteContext.UseCase);
                }

                concreteContext.ExecutedSignal.Set();
                concreteContext.FinishSignal.WaitOne();

                if (concreteContext.FinishMode.CompleteLogging)
                {
                    concreteContext.LoggingStrategy.Complete(loggingSession);
                }
            }
            finally
            {
                concreteContext.LoggingStrategy.Close(loggingSession);
            }
        }

        private sealed class LogWorkerContext
        {
            public TrackedUseCase UseCase { get; set; }
            public IOperationLoggingStrategy LoggingStrategy { get; set; }
            public AutoResetEvent ExecutedSignal { get; set; }
            public ManualResetEvent FinishSignal { get; set; }
            public LogWorkerFinishMode FinishMode { get; set; }
            public LogWorkerResult Result { get; set; }
        }

        private sealed class LogWorkerFinishMode
        {
            public bool CompleteLogging { get; set; }
        }

        private sealed class LogWorkerResult
        {
            public bool Succeeded { get; set; }
            public string Report { get; set; }
        }
    }
}
