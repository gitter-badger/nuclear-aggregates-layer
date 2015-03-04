using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings
{
    public interface IIntegrationLocalizationSettings : ISettings
    {
        string BasicLanguage { get; }
        string ReserveLanguage { get; }
        string RegionalTerritoryLocaleSpecificWord { get; }
    }
}
