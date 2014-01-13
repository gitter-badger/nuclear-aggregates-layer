using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6398, "Добавление проверки продажи на пустой адрес")]
    public class Migration6398 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int GroupId = 1;
            const int RuleId = 34;
            var script = string.Format(
                "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})", GroupId, RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
