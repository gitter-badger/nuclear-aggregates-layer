using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12809, "Откат миграций 12443 и 12528")]
    public sealed class Migration12809 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var branchOfficesTable = context.Database.GetTable(ErmTableNames.BranchOffices);

            branchOfficesTable.RemoveField("IsClosed");
            branchOfficesTable.RemoveField("ClosedOn");

            branchOfficesTable.Alter();

            const long Id = 203300498118314241;
            const int GeneralGroupId = 1;
            const int OrderSignupDateMoreThanBranchOfficeClosedDateRuleCode = 48;

            const string Query = @"DELETE FROM [Billing].[OrderValidationRuleGroupDetails] 
WHERE Id = {0} AND OrderValidationGroupId = {1} AND RuleCode = {2}";

            context.Database.ExecuteNonQuery(string.Format(
                Query,
                Id,
                GeneralGroupId,
                OrderSignupDateMoreThanBranchOfficeClosedDateRuleCode));
        }
    }
}