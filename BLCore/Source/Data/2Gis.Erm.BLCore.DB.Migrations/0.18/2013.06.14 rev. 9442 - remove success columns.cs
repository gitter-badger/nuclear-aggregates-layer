using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9442, "Удаление столбцов Success")]
    public class Migration9442 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            RemoveColumn(context, ErmTableNames.ExportFlowOrdersAdvMaterial);
            RemoveColumn(context, ErmTableNames.ExportFlowOrdersResource);
            RemoveColumn(context, ErmTableNames.ExportFlowOrdersTheme);
            RemoveColumn(context, ErmTableNames.ExportFlowOrdersThemeBranch);
            RemoveColumn(context, ErmTableNames.ExportFlowFinancialDataLegalEntity);
            RemoveColumn(context, ErmTableNames.ExportFlowOrdersOrder);
            RemoveColumn(context, ErmTableNames.ExportFlowCardExtensionsCardCommercial);
            RemoveColumn(context, ErmTableNames.ImportedFirmAddresses);
        }

        private void RemoveColumn(IMigrationContext context, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            var column = table.Columns["Success"];
            if (column != null)
            {
                column.Drop();
            }
        }
    }
}
