using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12469, "Добавляем проверку на региональное API")]
    public class Migration12469 : TransactedMigration
    {
        private const int GeneralValidationRule = 1;
        private const int RegionalApiOrderValidationRuleCode = 48;
        private const long PregeneratedId = 203066420941029377;
        private const string InsertStatements = @"
IF NOT EXISTS (SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] WHERE OrderValidationGroupId = {0} AND RuleCode = {1})
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES ({2}, {0}, {1})
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(InsertStatements, GeneralValidationRule, RegionalApiOrderValidationRuleCode, PregeneratedId));
        }
    }
}