using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationLogger : IOperationLogger
    {
        private readonly IEnumerable<IOperationLoggingStrategy> _loggingStrategies;
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

        public void Log(OperationScopeNode scopeNode)
        {
            var useCase = new TrackedUseCase
            {
                Description = string.Empty,
                RootNode = scopeNode
            };

            foreach (var strategy in _loggingStrategies)
            {
                string report;

                // TODO {all, 05.12.2013}: подумать, возможно, нужно добавить компенсационную логику, для уже успешно отработавших стратегий, возможно logging scope с rollback, complete, пока оставлен простой вариант
                if (!strategy.TryLogUseCase(useCase, out report))
                {
                    string msg = string.Format("Can't log usecase {0}, using strategy {1}", useCase, strategy.GetType().Name);
                    _logger.FatalEx(msg);
                    throw new InvalidOperationException(msg);
                }
            }
        }
    }
}
