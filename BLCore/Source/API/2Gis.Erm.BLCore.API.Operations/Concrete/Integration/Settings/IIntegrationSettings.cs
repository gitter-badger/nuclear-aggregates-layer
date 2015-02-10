using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings
{
    public interface IIntegrationSettings : ISettings
    {
        bool EnableIntegration { get; }
        bool UseWarehouseIntegration { get; }
        string IntegrationApplicationName { get; }
    }
}