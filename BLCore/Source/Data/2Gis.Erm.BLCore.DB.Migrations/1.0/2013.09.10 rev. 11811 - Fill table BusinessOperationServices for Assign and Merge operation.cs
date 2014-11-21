using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11811, "Выгрузка клиента (ExportFlowFinancialDataClient) при Assign и Merge для юл лица клиента и клиента")]
    public class Migration11811 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(InsertStatementTemplate, 942141479, 4, 11));  // LegalPerson, Assign, ExportFlowFinancialDataClient
            context.Connection.ExecuteNonQuery(string.Format(InsertStatementTemplate, 942141479, 23, 11));  // LegalPerson, Merge, ExportFlowFinancialDataClient
            context.Connection.ExecuteNonQuery(string.Format(InsertStatementTemplate, 1186303282, 4, 11));  // Client, Assign, ExportFlowFinancialDataClient
            context.Connection.ExecuteNonQuery(string.Format(InsertStatementTemplate, 1186303282, 23, 11));  // Client, Merge, ExportFlowFinancialDataClient
        }
    }
}
