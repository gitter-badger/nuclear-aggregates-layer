namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers
{
    public interface IMessageTransformer
    {
        bool CanTransform(IMessage originalMessage);
        IMessage Transform(IMessage originalMessage);
    }
}