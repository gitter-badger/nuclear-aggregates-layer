using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13740, "Отключаем проверку RegionalApiAdvertisementsOrderValidationRule")]
    public class Migration13740 : TransactedMigration
    {
        private const string DeleteStatement = "delete from [Billing].[OrderValidationRuleGroupDetails] where Id = 203066420941029377";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(DeleteStatement);
        }
    }
}