using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIServiceSettingsInitializer
    {
        Type ConcreteSettingsInterface { get; }
        bool TryInitialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings);
    }
}