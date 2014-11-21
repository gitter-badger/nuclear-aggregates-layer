using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.UI.WPF.Infrastructure.Presentation.Controls.Dialogs;
using DoubleGis.Erm.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.UI.WPF.Infrastructure.MVVM;
using DoubleGis.UI.WPF.Infrastructure.Messaging;
using DoubleGis.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.UI.WPF.Client.UseCases.Handlers.Actions
{
    public class FinishActionHandler : UseCaseSyncMessageHandlerBase<ExecuteActionMessage>
    {
        private readonly IDocumentManager _documentManager;

        public FinishActionHandler(IDocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        protected override bool ConcreteCanHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            return message.NeedConfirmation && message.Handled;
        }

        protected override IMessageProcessingResult ConcreteHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            foreach (var doc in useCase.State.NodesSnapshot.Select(x => x.Context).OfType<IDocument>())
            {
                foreach (var dialog in useCase.State.DialogsByMessages.Where(NeedConfirmation).Select(x => x.Value))
                {
                    if (string.IsNullOrEmpty(message.ErrorMessage))
                    {
                        _documentManager.RemoveDialog(doc, dialog);
                    }
                    else
                    {
                        dialog.ErrorMessage = message.ErrorMessage;
                    }
                }
            }

            useCase.State.DialogsByMessages.Remove(message);

            return EmptyResult;
        }

        private static bool NeedConfirmation(KeyValuePair<IMessage, IDialogViewModel> keyValuePair)
        {
            var message = keyValuePair.Key as ExecuteActionMessage;
            return message != null && message.NeedConfirmation;
        }
    }
}