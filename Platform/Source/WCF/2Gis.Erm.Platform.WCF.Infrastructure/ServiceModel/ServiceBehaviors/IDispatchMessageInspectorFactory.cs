using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    public interface IDispatchMessageInspectorFactory
    {
        IDispatchMessageInspector Create();
    }
}
