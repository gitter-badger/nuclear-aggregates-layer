using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors
{
    public interface IMessageFlowProcessorFactory
    {
        IAsyncMessageFlowProcessor CreateAsync<TMessageFlowProcessorSettings>(IMessageFlow messageFlow, TMessageFlowProcessorSettings messageFlowProcessorSettings)
            where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings;

        ISyncMessageFlowProcessor CreateSync<TMessageFlowProcessorSettings>(IMessageFlow messageFlow, TMessageFlowProcessorSettings messageFlowProcessorSettings)
            where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings;
    }
}