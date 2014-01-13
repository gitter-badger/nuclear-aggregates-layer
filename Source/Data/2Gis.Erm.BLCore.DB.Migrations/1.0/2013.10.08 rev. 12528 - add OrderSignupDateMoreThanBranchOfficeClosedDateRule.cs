using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12528, "Добавление проверки OrderSignupDateMoreThanBranchOfficeClosedDateRule")]
    public sealed class Migration12528 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const long Id = 203300498118314241;

            const int GeneralGroupId = 1;
            const int OrderSignupDateMoreThanBranchOfficeClosedDateRuleCode = 48;

            const string Query = @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] WHERE OrderValidationGroupId = {0} AND RuleCode = {1})
                                   INSERT INTO  [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES ({2}, {0}, {1})";

            context.Database.ExecuteNonQuery(string.Format(
                Query, 
                GeneralGroupId,
                OrderSignupDateMoreThanBranchOfficeClosedDateRuleCode, 
                Id));
        }
    }
}