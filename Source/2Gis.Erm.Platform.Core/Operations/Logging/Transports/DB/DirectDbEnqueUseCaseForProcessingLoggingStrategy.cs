using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class DirectDBEnqueUseCaseForProcessingLoggingStrategy : IOperationLoggingStrategy, ISimplifiedModelConsumer
    {
        private readonly IOperationsPrimaryProcessingEnqueAggregateService _operationsPrimaryProcessingEnqueAggregateService;
        private readonly IReadOnlyCollection<IMessageFlow> _primaryProcessingFlows;

        public DirectDBEnqueUseCaseForProcessingLoggingStrategy(
            IMessageFlowRegistry messageFlowRegistry,
            IOperationsPrimaryProcessingEnqueAggregateService operationsPrimaryProcessingEnqueAggregateService)
        {
            _operationsPrimaryProcessingEnqueAggregateService = operationsPrimaryProcessingEnqueAggregateService;
            
            // FIXME {i.maslennikov, 25.08.2014}: По-сути, сейчас нет возможности выполнить primary processing, не указав интерфейс IPerformedOperationsPrimaryProcessingFlow для потока.
            //                                    Но сильно неочевидно, что это нужно сделать. Оно просто и тихо не будет работать, так?
            _primaryProcessingFlows = messageFlowRegistry.Flows.Where(f => f is IPerformedOperationsPrimaryProcessingFlow).ToArray();
        }

        public bool TryLogUseCase(TrackedUseCase useCase, out string report)
        {
            report = null;

            if (_primaryProcessingFlows.Count == 0)
            {
                return true;
            }

            _operationsPrimaryProcessingEnqueAggregateService.Push(useCase.Id, _primaryProcessingFlows);

            return true;
        }
    }
}