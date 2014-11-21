using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(16472, "Добавление проверки на наличие сопутствующие/запрещенные при смене статуса заказа", "d.ivanov")]
    public class Migration16472 : TransactedMigration
    {
        private const long Id = 295334265523012353;
        private const int GroupId = 3;
        private const int RuleId = 49;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var script = string.Format(
                "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId,RuleCode) VALUES ({0}, {1}, {2})",
                Id,
                GroupId,
                RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
