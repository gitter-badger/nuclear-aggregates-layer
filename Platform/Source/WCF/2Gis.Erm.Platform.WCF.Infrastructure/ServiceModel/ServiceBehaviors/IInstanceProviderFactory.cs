using System;
using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    public interface IInstanceProviderFactory
    {
        IInstanceProvider Create(Type serviceType);
    }
}
