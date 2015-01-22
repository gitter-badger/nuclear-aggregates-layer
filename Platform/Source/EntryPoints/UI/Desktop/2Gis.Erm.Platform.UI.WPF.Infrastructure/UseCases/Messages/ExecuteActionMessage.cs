using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class ExecuteActionMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly StrictOperationIdentity _operation;
        private readonly Guid _actionHostId;

        public ExecuteActionMessage(StrictOperationIdentity operation, Guid actionHostId)
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

        public bool NeedConfirmation { get; set; }
        public bool Confirmed { get; set; }
    }
}