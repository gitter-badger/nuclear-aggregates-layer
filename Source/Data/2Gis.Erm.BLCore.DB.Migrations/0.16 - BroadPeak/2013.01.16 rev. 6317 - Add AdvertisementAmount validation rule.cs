using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6317, "Добавление проверки количества рекламы")]
    public class Migration6317 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int GroupId = 4;
            const int RuleId = 26;
            var script = string.Format(
                "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})", GroupId, RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
