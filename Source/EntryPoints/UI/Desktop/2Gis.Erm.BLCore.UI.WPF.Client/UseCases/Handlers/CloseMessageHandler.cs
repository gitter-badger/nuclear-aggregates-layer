using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class CloseMessageHandler : UseCaseSyncMessageHandlerBase<CloseMessage>
    {
        private readonly IDocumentManager _documentManager;

        public CloseMessageHandler(IDocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        protected override IMessageProcessingResult ConcreteHandle(CloseMessage message, IUseCase useCase)
        {
            IViewModel viewModel;
            IUseCaseNode useCaseNode;
            if (!useCase.TryGetViewModelById(message.CloseTargetId, out viewModel, out useCaseNode))
            {
                return null;
            }

            var context = useCaseNode.Context;
            var targetDocument = context as IDocument;
            if (targetDocument != null)
            {
                useCase.State.Rollback(useCaseNode.Id);
                _documentManager.Remove(targetDocument);
                return EmptyResult;
            }

            var targetDialog = context as IDialog;
            if (targetDialog != null)
            {
                useCase.State.Rollback(useCaseNode.Id);
                foreach (var document in useCase.State.NodesSnapshot.Select(x => x.Context).OfType<IDocument>())
                {
                    _documentManager.RemoveDialog(document, targetDialog);
                }

                return EmptyResult;
            }

            var containerDescription = useCaseNode.Context != null
                ? useCaseNode.Context.GetType().Name
                : "Empty context";

            throw new InvalidOperationException(string.Format("Unsupported container type {0}. Container have to be document or dialog", containerDescription));
        }
    }
}