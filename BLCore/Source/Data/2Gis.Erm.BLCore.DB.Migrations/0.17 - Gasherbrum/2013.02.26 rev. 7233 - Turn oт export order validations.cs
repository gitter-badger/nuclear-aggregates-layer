using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7233, "Включаем проверки экспорта")]
    public sealed class Migration7233 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId, RuleCode) VALUES (4, 26), (1, 27), (1, 28), (1, 29), (1, 30), (4, 31), (1, 32), (1, 33), (1, 34), (1, 35), (1, 36), (1, 37), (1, 38), (1, 39)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
