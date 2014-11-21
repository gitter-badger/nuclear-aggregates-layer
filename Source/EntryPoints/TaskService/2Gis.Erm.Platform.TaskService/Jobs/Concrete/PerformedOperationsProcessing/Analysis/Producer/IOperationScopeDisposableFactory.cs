using System;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    /// <summary>
    /// Абстракция была добавлена чтобы обеспечить более строгий контракт для IOperationScopeFactory - там где необходима поддержка dispose,
    /// В некоторых сценариях это важно, т.к. с точки зрения lifetime требуется строгое perscope поведение, при этом scope может быть вложенным
    /// </summary>
    public interface IOperationScopeDisposableFactory : IOperationScopeFactory, IDisposable
    {
    }
}