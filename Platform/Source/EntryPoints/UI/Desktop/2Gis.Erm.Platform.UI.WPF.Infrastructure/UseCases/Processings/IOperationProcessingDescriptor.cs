using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings
{
    public interface IOperationProcessingDescriptor : IProcessingDescriptor
    {
        Guid? ProgressConsumerId { get; }
        int ProcessingItemsCount { get; }
        IOperationResult[] CurrentProgress { get; }
        bool IsFinished { get; }
        void UpdateOperationProgress(IOperationResult[] operationsResults);   
    }
}