using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationLogger : IOperationLogger
    {
        private readonly IReadOnlyCollection<IOperationLoggingStrategy> _loggingStrategies;
        private readonly ITracer _logger;

        public OperationLogger(IOperationLoggingStrategy[] loggingStrategies, // unity registrations 1..*
                               ITracer logger)
        {
            _loggingStrategies = loggingStrategies;
            _logger = logger;
        }

        public void Log(TrackedUseCase useCase)
        {
            // COMMENT {all, 29.07.2014}: пока оставлена реализация логирования строго последовательная, до ввода service bus в промышленную эксплуатацию, 
            //                            на случай, если с parallel возникнут, какие-то трудности
            SequentialLogging(useCase);

            // ParallelLogging(useCase);
        }

        private void SequentialLogging(TrackedUseCase useCase)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var loggingSessions = new List<Tuple<IOperationLoggingStrategy, LoggingSession>>();

            var isUseCaseLoggedSuccessfully = true;
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
                        _logger.ErrorFormat("Can't log use case {0} through strategy {1}. Details: {2}", useCase, strategy.GetType(), report);
                        isUseCaseLoggedSuccessfully = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat(ex, "Can't log use case {0} through strategy {1}", useCase, strategy.GetType());
                    isUseCaseLoggedSuccessfully = false;
                }
                finally
                {
                    strategyStopwatch.Stop();
                    _logger.DebugFormat("Logging strategy {0} executed. It takes {1} sec. Usecase details: {2}",
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
            _logger.DebugFormat("Sequential logging of use case {0} takes {1} sec", useCase, stopwatch.Elapsed.TotalSeconds);

            if (!isUseCaseLoggedSuccessfully)
            {
                var msg = "Can't log use case " + useCase + " logging aborted";
                _logger.Error(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private void ParallelLogging(TrackedUseCase useCase)
        {
            var stopwatch = new Stopwatch();
            var finishSignal = new ManualResetEvent(false);
            var logWorkerFinishMode = new LogWorkerFinishMode { CompleteLogging = true };

            WaitHandle[] loggingExecutedSignals;
            IEnumerable<LogWorkerResult> workerResults;

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
                    _logger.Error("One of the workers failed: " + result.Report);
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
                _logger.ErrorFormat(ex, "Can't log usecase {0}. One of the logging workers failed", useCase);
                logWorkerFinishMode.CompleteLogging = false;
            }

            finishSignal.Close();
            foreach (var signal in loggingExecutedSignals)
            {
                signal.Close();
            }

            stopwatch.Stop();
            _logger.DebugFormat("Parallel logging of use case {0} takes {1} sec", useCase, stopwatch.Elapsed.TotalSeconds);

            if (!logWorkerFinishMode.CompleteLogging)
            {
                var msg = "Can't log use case " + useCase + " logging aborted";
                _logger.Error(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private Task[] ResolveLogWorkers(TrackedUseCase useCase,
                                         ManualResetEvent finishSignal,
                                         LogWorkerFinishMode logWorkerFinishMode,
                                         out WaitHandle[] loggingExecutedSignals,
                                         out IEnumerable<LogWorkerResult> workersResults)
        {
            var workersActions = new List<Task>();
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

                workersActions.Add(new Task(LogWorker, context));
                signals.Add(executedSignal);
                results.Add(result);
            }

            loggingExecutedSignals = signals.ToArray();
            workersResults = results;
            return workersActions.ToArray();
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
                    _logger.Error(ex, msg);

                    concreteContext.Result.Report = msg;
                    concreteContext.Result.Succeeded = false;
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.DebugFormat("Logging strategy {0} executed. It takes {1} sec. Usecase details: {2}",
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
