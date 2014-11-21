using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8541, "Удаляет одно из правил проверки сопутствующих-запрещенных")]
    public sealed class Migration8541 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery("delete from Billing.OrderValidationRuleGroupDetails where RuleCode = 45");
        }
    }
}
