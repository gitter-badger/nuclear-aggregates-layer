namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors
{
    public interface ISyncMessageFlowProcessor : IMessageFlowProcessor
    {
        void Process();
    }
}