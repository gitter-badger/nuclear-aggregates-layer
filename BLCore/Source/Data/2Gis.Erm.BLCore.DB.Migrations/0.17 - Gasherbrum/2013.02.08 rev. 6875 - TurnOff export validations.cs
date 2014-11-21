using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6875, "Выключаем проверки, которые не должны попасть в релиз")]
    public sealed class Migration6875 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode IN (26, 27,28, 29, 30,
                                       31, 32, 33, 34, 35, 36, 37, 38, 39)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
