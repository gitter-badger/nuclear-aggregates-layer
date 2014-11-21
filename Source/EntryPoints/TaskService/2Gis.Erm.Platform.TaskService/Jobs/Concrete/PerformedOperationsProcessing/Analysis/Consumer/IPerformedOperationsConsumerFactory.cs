using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer
{
    public interface IPerformedOperationsConsumerFactory
    {
        IPerformedOperationsConsumer Create(
            Type performedOperationsSourceFlowType,
            PerformedOperationsTransport targetTransport,
            int batchSize,
            CancellationToken cancellationToken);
    }
}