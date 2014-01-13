namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings
{
    public interface IIntegrationSettings
    {
        bool EnableIntegration { get; }
        bool EnableRabbitMqQueue { get; }
        bool UseWarehouseIntegration { get; }
        string IntegrationApplicationName { get; }
    }
}