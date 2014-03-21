using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(17700, "Удаление отдельной проверки на наличие сопутствующие/запрещенные при смене статуса заказа. Логика интегрирована в существующую проверку", "d.ivanov")]
    public class Migration17700 : TransactedMigration
    {
        private const int GroupId = 3;
        private const int RuleId = 49;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var script = string.Format(
                "DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE OrderValidationGroupId = {0} AND RuleCode = {1}",
                GroupId,
                RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
