using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9338, "Добавляем правило на наличие заглушек РМ")]
    public sealed class Migration9338 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string Query = @"
IF NOT EXISTS(SELECT *  FROM [Billing].[OrderValidationRuleGroupDetails] where OrderValidationGroupId = 2 AND RuleCode = 47)
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES (66, 2, 47)

DELETE FROM Billing.OrderValidationResults WHERE OrderValidationGroupId = 2";

            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
