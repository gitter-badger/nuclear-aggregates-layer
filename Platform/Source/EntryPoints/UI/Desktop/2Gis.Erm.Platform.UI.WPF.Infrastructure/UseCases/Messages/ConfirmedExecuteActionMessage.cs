using DoubleGis.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.UI.WPF.Client.Common.Model.Usecase.Messages
{
    public class ConfirmedExecuteActionMessage : MessageBase<SequentialProcessingModel>
    {
        public ConfirmedExecuteActionMessage(ExecuteActionMessage baseMessage) : base(null)
        {
            BaseMessage = baseMessage;
        }

        public ExecuteActionMessage BaseMessage { get; set; }
    }
}