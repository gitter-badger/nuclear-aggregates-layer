using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IUseCaseState : IDisposable
    {
        bool IsEmpty { get; }
        IUseCaseNode Root { get; }
        IUseCaseNode Current { get; }

        IUseCaseNode[] NodesSnapshot { get; }

        IExecutingProcessingsRegistry ProcessingsRegistry { get; }

        bool TryMoveNext(object context);
        bool TryMoveNext(Guid previousNodeId, object context);
        bool Rollback(Guid targetNodeId);
    }
}
