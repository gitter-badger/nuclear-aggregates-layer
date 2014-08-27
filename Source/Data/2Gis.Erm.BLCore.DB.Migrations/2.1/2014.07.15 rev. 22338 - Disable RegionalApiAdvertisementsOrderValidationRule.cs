using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22338, "[ERM-4371] Отключить проверку: Проверка по региональной рекламе в API (RegionalApiAdvertisementsOrderValidationRule)", "i.maslennikov")]
    public class Migration22338 : TransactedMigration
    {
        private const int GroupId = 1;
        private const int RuleId = 48;

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
