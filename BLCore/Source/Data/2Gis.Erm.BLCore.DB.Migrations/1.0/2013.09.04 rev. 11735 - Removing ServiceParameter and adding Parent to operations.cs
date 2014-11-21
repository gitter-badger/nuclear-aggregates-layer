using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11735, "Удаление ServiceParameter в таблице [Shared].[BusinessOperationServices] и добавление колонки Parent в таблице [Shared].[PerformedBusinessOperations]")]
    public class Migration11735 : TransactedMigration
    {
        private const string ServiceParameterColumn = "ServiceParameter";
        private const string ParentColumn = "Parent";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var businessOperationServicesTable = context.Database.GetTable(ErmTableNames.BusinessOperationServices);
            if (businessOperationServicesTable.Columns.Contains(ServiceParameterColumn))
            {
                var column = businessOperationServicesTable.Columns[ServiceParameterColumn];
                column.Drop();
            }

            var performedBusinessOperationsTable = context.Database.GetTable(ErmTableNames.PerformedBusinessOperations);
            if (!performedBusinessOperationsTable.Columns.Contains(ParentColumn))
            {
                var column = new Column(performedBusinessOperationsTable, ParentColumn, DataType.BigInt) { Nullable = true };
                performedBusinessOperationsTable.Columns.Add(column);
                performedBusinessOperationsTable.Alter();
            }
        }
    }
}
