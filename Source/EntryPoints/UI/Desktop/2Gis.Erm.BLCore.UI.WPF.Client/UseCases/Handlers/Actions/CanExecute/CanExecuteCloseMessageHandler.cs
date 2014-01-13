using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.CanExecute
{
    public sealed class CanExecuteCloseMessageHandler : UseCaseSyncMessageHandlerBase<CanExecuteActionMessage>
    {
        private readonly IDocumentManager _documentManager;

        public CanExecuteCloseMessageHandler(IDocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        protected override bool ConcreteCanHandle(CanExecuteActionMessage message, IUseCase useCase)
        {
            var targetOperation = message.Operations.FirstOrDefault();
            return targetOperation != null && targetOperation.Identity.Equals(CloseIdentity.Instance) && !useCase.State.IsEmpty;
        }

        protected override IMessageProcessingResult ConcreteHandle(CanExecuteActionMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return CanExecuteResult.False;
            }

            var document = currentElement.Context as IDocument;
            return document != null && _documentManager.Documents.Any() ? CanExecuteResult.True : CanExecuteResult.False; 
        }
    }
}