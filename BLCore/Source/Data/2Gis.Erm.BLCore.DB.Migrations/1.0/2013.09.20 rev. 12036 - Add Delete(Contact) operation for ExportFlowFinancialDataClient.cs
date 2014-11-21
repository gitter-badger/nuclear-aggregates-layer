using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12036, "Выгрузка клиента по факту операции удаления контактного лица")]
    public class Migration12036 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 1797543439, 9, 11)); // Contact, Delete, ExportFlowFinancialDataClient
        }
    }
}