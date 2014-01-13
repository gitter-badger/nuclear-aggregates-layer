using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8322, "Новая проверка")]
    public sealed class Migration8322 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] where RuleCode = 46)
  INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES (64, 1, 46)"); // Id с боевой базы
        }
    }
}
