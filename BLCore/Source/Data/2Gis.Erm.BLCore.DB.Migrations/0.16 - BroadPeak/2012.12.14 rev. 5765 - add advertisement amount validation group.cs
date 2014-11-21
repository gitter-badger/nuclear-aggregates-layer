using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5765, "Добавляем группу проверок Количество рекламы")]
    public class Migration5765 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int GroupCode = 4;

            var query =
                string.Format(
                    @" IF NOT Exists (SELECT * FROM [Billing].[OrderValidationRuleGroups] WHERE Code = {0}) 
                                    INSERT INTO [Billing].[OrderValidationRuleGroups] (Code) VALUES ({0})",
                    GroupCode);

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
