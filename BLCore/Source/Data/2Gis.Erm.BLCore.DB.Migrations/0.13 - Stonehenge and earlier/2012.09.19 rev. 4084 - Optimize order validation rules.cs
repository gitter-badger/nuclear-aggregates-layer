using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4084, "Оптимизация проверок рекламных материалов")]
    public sealed class Migration4084 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // CouponPeriodIsCorrect = 2
            // AdvertisementsElementsNotEmpty = 4
            // AllRequiredAdvertisementsAttached = 5
            context.Database.ExecuteNonQuery("DELETE FROM Billing.OrderValidationRuleGroupDetails WHERE RuleCode IN (2, 4, 5, 19)");

            // AdvertisementMaterialsValidation = 2
            // ValidateAdvertisements = 22
            context.Database.ExecuteNonQuery("INSERT INTO Billing.OrderValidationRuleGroupDetails(OrderValidationGroupId, RuleCode) VALUES (2, 22) ");
        }
    }
}
