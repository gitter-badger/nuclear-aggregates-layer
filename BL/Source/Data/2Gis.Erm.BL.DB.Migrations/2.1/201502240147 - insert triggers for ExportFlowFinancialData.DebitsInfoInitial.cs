using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201502240147, "Триггеры на выгрузку ExportFlowFinancialData.DebitsInfoInitial", "a.rechkalov")]
    public class Migration201502240147 : TransactedMigration
    {
        private const string StatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Operation = {0} AND Descriptor = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Operation, Descriptor, Service)
VALUES ({0}, {1}, {2})";

        private const int ExportService = 19;
        private const int WithdrawFromAccountsIdentity = 1015;
        private const int RevertWithdrawFromAccountsIdentity = 1018;
        private const int Descriptor = 0;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(StatementTemplate, WithdrawFromAccountsIdentity, Descriptor, ExportService));
            context.Database.ExecuteNonQuery(string.Format(StatementTemplate, RevertWithdrawFromAccountsIdentity, Descriptor, ExportService));
        }
    }
}