using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages
{
    public sealed class CloseMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly Guid _closeTargetId;

        public CloseMessage(Guid closeTargetId) 
            : base(null)
        {
            _closeTargetId = closeTargetId;
        }

        public Guid CloseTargetId
        {
            get { return _closeTargetId; }
        }
    }
}
