using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class ExecuteActionMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly IBoundOperationFeature[] _operations;
        private readonly Guid _actionHostId;

        public ExecuteActionMessage(IBoundOperationFeature[] operations, Guid actionHostId)
            : base(null)
        {
            _operations = operations;
            _actionHostId = actionHostId;
        }

        public IBoundOperationFeature[] Operations
        {
            get { return _operations; }
        }
        
        public Guid ActionHostId
        {
            get { return _actionHostId; }
        }

        public bool NeedConfirmation { get; set; }
        public bool Confirmed { get; set; }
    }
}