using DoubleGis.Erm.BLCore.API.Aggregates.Settings;

using Nuclear.Settings;
using Nuclear.Settings.API;

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