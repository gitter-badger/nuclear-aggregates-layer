using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11089, "Перевод времени в таблицах Export* из местного часового пояса в UTC")]
    public sealed class Migration11089 : TransactedMigration
    {
        private const string QueryTemplate = "update [{0}].[{1}] set [Date] = DATEADD(hh, {2}, [Date])";
        private const int LocalToUtcOffset = -7;
        private static readonly SchemaQualifiedObjectName[] Tables = new[]
            {
                ErmTableNames.ExportFlowCardExtensionsCardCommercial,
                ErmTableNames.ExportFlowFinancialDataClient,
                ErmTableNames.ExportFlowFinancialDataLegalEntity,
                ErmTableNames.ExportFlowOrdersAdvMaterial,
                ErmTableNames.ExportFlowOrdersOrder,
                ErmTableNames.ExportFlowOrdersResource,
                ErmTableNames.ExportFlowOrdersTheme,
                ErmTableNames.ExportFlowOrdersThemeBranch,
                ErmTableNames.PerformedBusinessOperations,
                ErmTableNames.ImportedFirmAddresses,
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var table in Tables)
            {
                var query = string.Format(QueryTemplate, table.Schema, table.Name, LocalToUtcOffset);
                context.Connection.ExecuteNonQuery(query);
            }
        }
    }
}
