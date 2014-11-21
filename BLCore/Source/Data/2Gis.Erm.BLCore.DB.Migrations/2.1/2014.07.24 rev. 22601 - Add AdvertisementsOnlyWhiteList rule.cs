using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22601, "Добавление проверки AdvertisementsOnlyWhiteListOrderValidationRule - из-за проблем с производительностью старый ValidateAdvertisementsOrderValidationRule был разделен", "i.maslennikov")]
    public class Migration16472 : TransactedMigration
    {
        private const long Id = 412536604214531016;
        private const int GroupId = 2;
        private const int RuleId = 21;

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
