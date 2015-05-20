using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Operations.Identity;

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