using System;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages
{
    public sealed class CanExecuteActionMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly StrictOperationIdentity _operation;
        private readonly Guid _actionHostId;

        public CanExecuteActionMessage(StrictOperationIdentity operation, Guid actionHostId)
            : base(null)
        {
            _operation = operation;
            _actionHostId = actionHostId;
        }

        public StrictOperationIdentity Operation
        {
            get { return _operation; }
        }

        public Guid ActionHostId
        {
            get { return _actionHostId; }
        }
    }
}