using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.Aggregates.Settings
{
    public sealed class DebtProcessingSettingsAspect : ISettingsAspect, IDebtProcessingSettings
    {
        private readonly DecimalSetting _minDebtAmount = ConfigFileSetting.Decimal.Required("MinDebtAmount");

        public decimal MinDebtAmount 
        {
            get { return _minDebtAmount.Value; }
        }
    }
}