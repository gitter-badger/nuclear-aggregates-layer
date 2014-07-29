using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11054, "Отключаем проверку AdvertisementTextFormattingOrderValidationRule")]
    public sealed class Migration11054 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int advertisementTextFormattingOrderValidationRuleCode = 28;
            context.Connection.ExecuteNonQuery(
                string.Format(
                    @"DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode = {0}",
                    advertisementTextFormattingOrderValidationRuleCode));
        }
    }
}
