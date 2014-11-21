namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public enum MessageProcessingStage
    {
        None = 0,
        Received = 10,
        Split = 20,
        Validation = 30,
        Transforming = 40,
        Processing = 50,
        Handle = 60
    }
}