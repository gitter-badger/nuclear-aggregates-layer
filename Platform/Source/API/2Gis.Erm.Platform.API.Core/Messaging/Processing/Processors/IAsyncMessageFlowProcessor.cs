namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors
{
    public interface IAsyncMessageFlowProcessor : IMessageFlowProcessor
    {
        void Start();
        void Stop();
        void Wait();
    }
}