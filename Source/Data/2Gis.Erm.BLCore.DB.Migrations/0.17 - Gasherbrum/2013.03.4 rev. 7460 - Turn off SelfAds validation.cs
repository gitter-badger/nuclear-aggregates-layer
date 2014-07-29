using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7460, "Выключаем проверку на саморекламу")]
    public sealed class Migration7460 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"
DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode = 36";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
