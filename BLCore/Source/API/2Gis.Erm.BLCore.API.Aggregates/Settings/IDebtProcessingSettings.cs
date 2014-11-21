using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Settings
{
    public interface IDebtProcessingSettings : ISettings
    {
        decimal MinDebtAmount { get; }
    }
}