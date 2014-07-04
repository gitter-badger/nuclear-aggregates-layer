namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators
{
    public interface IMessageValidator
    {
        bool CanValidate(IMessage message);
        bool IsValid(IMessage message, out string report);
    }
}