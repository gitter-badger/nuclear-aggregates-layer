using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.Aggregates.Settings
{
    public interface IDebtProcessingSettings : ISettings
    {
        decimal MinDebtAmount { get; }
    }
}
