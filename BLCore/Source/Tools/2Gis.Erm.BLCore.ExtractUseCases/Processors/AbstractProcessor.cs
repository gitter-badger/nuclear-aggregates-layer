using NuClear.Storage.UseCases;

using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IProcessingContext _processingContext;
        private readonly IWorkspace _workspace;
        private readonly ISolution _solution;

        protected AbstractProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
        {
            _processingContext = processingContext;
            _workspace = workspace;
            _solution = solution;
        }

        protected ISolution Solution
        {
            get
            {
                return _solution;
            }
        }
        protected IProcessingContext ProcessingContext
        {
            get
            {
                return _processingContext;
            }
        }
        protected IWorkspace Workspace
        {
            get
            {
                return _workspace;
            }
        }

        public abstract bool IsDocumentProcessingRequired(ISemanticModel semanticModel, IDocument document);
        public abstract void ProcessDocument(ISemanticModel semanticModel, IDocument document);
        public abstract void FinishProcessing();
    }
}