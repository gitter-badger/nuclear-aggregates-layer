using System;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings
{
    public interface IExecutingProcessingsRegistry
    {
        IProcessingDescriptor StartProcessing(IProcessingDescriptor processingDescriptor);
        IProcessingDescriptor FinishProcessing(Guid processingId);

        IProcessingDescriptor[] Processings { get; }
    }
}
