using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7076, "Выключаем проверки тематик")]
    public sealed class Migration7076 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode IN (40, 41, 42, 43, 44)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
