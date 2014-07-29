using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl 
{
    [Migration(12172, "Добавляем обработку MergeIdentity<LegalPerson> для ExportFlowFinancialDataLegalEntity, ExportFlowOrdersOrder, ExportFlowFinancialDataClient")]
    public class Migration12172 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 942141479, 25, 3));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 942141479, 25, 5));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 942141479, 25, 11));
        }
    }
}