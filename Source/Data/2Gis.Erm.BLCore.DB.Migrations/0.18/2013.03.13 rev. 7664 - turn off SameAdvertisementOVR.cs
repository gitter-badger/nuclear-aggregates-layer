using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7664, "Выключаем проверку SameAdvertisementOrderValidationRule")]
    public sealed class Migration7664 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"DELETE FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode = 21";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
