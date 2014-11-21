using System;
using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public interface IServiceClientSettingsProvider
    {
        ServiceEndpoint GetEndpoint(Type contractType, Type bindingType);
    }
}