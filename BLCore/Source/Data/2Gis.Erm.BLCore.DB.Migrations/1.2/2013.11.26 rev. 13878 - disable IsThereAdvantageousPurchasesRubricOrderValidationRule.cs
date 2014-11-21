using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13878, "Отключаем проверку IsThereAdvantageousPurchasesRubricOrderValidationRule")]
    public class Migration13878 : TransactedMigration
    {
        private const string DeleteStatement = "delete from [Billing].[OrderValidationRuleGroupDetails] where Id = 50 and RuleCode = 30";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(DeleteStatement);
        }
    }
}