using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Settings
{
    public interface IDebtProcessingSettings : ISettings
    {
        decimal MinDebtAmount { get; }
    }
}