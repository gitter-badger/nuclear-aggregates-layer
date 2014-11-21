namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public sealed class SequentialProcessingModel : IMessageProcessingModel
    {
        public ProcessingModel MessageProcessingModel
        {
            get
            {
                return ProcessingModel.Sequential;
            }
        }
    }
}