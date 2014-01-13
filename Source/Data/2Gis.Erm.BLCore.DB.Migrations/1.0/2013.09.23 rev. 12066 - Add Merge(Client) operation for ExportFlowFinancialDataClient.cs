using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12066, "Выгрузка клиента по факту слияния клиентов")]
    public class Migration12066 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 1186303282, 25, 11)); // Client, Merge, ExportFlowFinancialDataClient
        }
    }
}
