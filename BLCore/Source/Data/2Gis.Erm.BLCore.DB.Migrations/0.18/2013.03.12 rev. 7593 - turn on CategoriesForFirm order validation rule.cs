using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7593, "Включаем проверку на кол-во категорий для фирмы")]
    public sealed class Migration7593 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"IF NOT EXISTS(SELECT * FROM Billing.OrderValidationRuleGroupDetails WHERE RuleCode = 32)
INSERT INTO Billing.OrderValidationRuleGroupDetails(OrderValidationGroupId, RuleCode) Values(1, 32)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
