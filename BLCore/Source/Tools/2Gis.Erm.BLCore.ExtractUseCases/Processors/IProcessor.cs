using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors
{
    public interface IProcessor
    {
        bool IsDocumentProcessingRequired(ISemanticModel symanticModel, IDocument document);
        void ProcessDocument(ISemanticModel symanticModel, IDocument document);
        void FinishProcessing();
    }
}