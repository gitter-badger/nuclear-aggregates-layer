namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public sealed class FreeProcessingModel : IMessageProcessingModel
    {
        public ProcessingModel MessageProcessingModel
        {
            get
            {
                return ProcessingModel.Free;
            }
        }
    }
}