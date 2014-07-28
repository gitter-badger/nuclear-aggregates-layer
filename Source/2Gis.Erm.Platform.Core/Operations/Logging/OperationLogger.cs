using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

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
            IReadOnlyCollection<DependentTransaction> dependentTransactions;
            var workers = ResolveLogWorkers(useCase, out dependentTransactions);

            bool useCaseLoggedSuccessfully = true;

            try
            {
                try
                {
                    foreach (var worker in workers)
                    {
                        worker.Start();
                    }

                    Task.WaitAll(workers);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't log usecase {0}. One of the logging workers failed", useCase);
                    throw;
                }

                foreach (var worker in workers)
                {
                    try
                    {
                        var workerResult = worker.Result;
                        if (!workerResult.Succeeded)
                        {
                            _logger.ErrorEx("One of the workers failed: " + workerResult.Report);
                            useCaseLoggedSuccessfully = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormatEx(ex, "Can't log usecase {0}. Error was caught while worker result evaluating", useCase);
                        throw;
                    }
                }
            }
            finally
            {
                foreach (var transaction in dependentTransactions)
                {
                    if (useCaseLoggedSuccessfully)
                    {
                        transaction.Complete();
                    }

                    transaction.Dispose();
                }
            }

            if (!useCaseLoggedSuccessfully)
            {
                throw new InvalidOperationException("Can't log use case " + useCase);
            }
        }

        private Task<LogWorkerResult>[] ResolveLogWorkers(
            TrackedUseCase useCase, 
            out IReadOnlyCollection<DependentTransaction> dependentTransactions)
        {
            var workers = new List<Task<LogWorkerResult>>();
            var transactions = new List<DependentTransaction>();

            foreach (var strategy in _loggingStrategies)
            {
                var dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
                var context = new LogWorkerContext
                    {
                        UseCase = useCase,
                        DependentTransaction = dependentTransaction,
                        LoggingStrategy = strategy
                    };

                workers.Add(new Task<LogWorkerResult>(LogWorker, context));
                transactions.Add(dependentTransaction);
            }

            dependentTransactions = transactions;
            return workers.ToArray();
        }

        private LogWorkerResult LogWorker(object context)
        {
            var concreteContext = (LogWorkerContext)context;
            var result = 
                new LogWorkerResult
                    {
                        UsedTransaction = concreteContext.DependentTransaction
                    };

            try
            {
                using (var transaction = new TransactionScope(concreteContext.DependentTransaction, DefaultTransactionOptions.Default.Timeout))
                {
                    string report;
                    var isSuccessfullyLogged = concreteContext.LoggingStrategy.TryLogUseCase(concreteContext.UseCase, out report);
                    result.Report = report;
                    result.Succeeded = isSuccessfullyLogged;

                    if (isSuccessfullyLogged)
                    {
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Can't log use case {0} properly through logging strategy {1}", concreteContext.UseCase, concreteContext.LoggingStrategy.GetType());
                _logger.ErrorEx(ex, msg);
                result.Report = msg;
            }

            return result;
        }

        private sealed class LogWorkerContext
        {
            public TrackedUseCase UseCase { get; set; }
            public IOperationLoggingStrategy LoggingStrategy { get; set; }
            public DependentTransaction DependentTransaction { get; set; }
        }

        private sealed class LogWorkerResult
        {
            public bool Succeeded { get; set; }
            public string Report { get; set; }
            public DependentTransaction UsedTransaction { get; set; }
        }
    }
}
