using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Execute
{
    public sealed class ExecuteCloseMessageHandler : UseCaseSyncMessageHandlerBase<ExecuteActionMessage>
    {
        private readonly IDocumentManager _documentManager;

        public ExecuteCloseMessageHandler(IDocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        protected override bool ConcreteCanHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            var targetOperation = message.Operations.FirstOrDefault();
            return targetOperation != null && targetOperation.Identity.Equals(CloseIdentity.Instance) && !useCase.State.IsEmpty;
        }
        
        protected override IMessageProcessingResult ConcreteHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var document = currentElement.Context as IDocument;
            if (document != null)
            {
                _documentManager.Remove(document);
            }

            return EmptyResult;
        }
    }
}