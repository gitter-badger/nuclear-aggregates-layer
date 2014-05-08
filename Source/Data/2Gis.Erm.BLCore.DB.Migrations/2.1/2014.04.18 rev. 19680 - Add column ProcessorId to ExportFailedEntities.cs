using System;
using System.Collections.Generic;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(19680, "Добавление колонки ProcessorId в таблицу [Integration].[ExportFailedEntities]", "i.maslennikov")]
    public class Migration19680 : TransactedMigration
    {
        private const string TargetColumnName = "ProcessorId";
        private const string TargetColumnDefaultConstraintName = "DF_" + TargetColumnName;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ExportFailedEntities);
            if (table.Columns.Contains(TargetColumnName))
            {
                return;
            }

            InsertColumn(context.Database, table);
            UpdateData(context.Database);
            Complete(context);
        }

        private void InsertColumn(Database database, Table table)
        {
            int targetColumnIndex = table.Columns.Count;
            var columnsToInsert = new[] { new InsertedColumnDefinition(targetColumnIndex, ColumnCreator) };
            EntityCopyHelper.CopyAndInsertColumns(database, table, columnsToInsert);
        }

        private Column ColumnCreator(SqlSmoObject smo)
        {
            var column = new Column(smo, TargetColumnName, DataType.Int) { Nullable = false };
            column.AddDefaultConstraint(TargetColumnDefaultConstraintName).Text = "0";
            return column;
        }

        private void UpdateData(Database database)
        {
            var processor2EntitiesMap = new Dictionary<int, int>
                {
                    { /*ExportFlowCardExtensionsCardCommercial*/ 246, /*FirmAddress =*/ 164 },
                    { /*ExportFlowFinancialDataLegalEntity*/ 247, /*LegalPerson =*/ 147 },
                    { /*ExportFlowOrdersAdvMaterial*/ 248, /*Advertisement =*/ 186 },
                    { /*ExportFlowOrdersOrder*/ 249, /*Order =*/ 151 },
                    { /*ExportFlowOrdersResource*/ 250, /*ThemeTemplate =*/ 222 },
                    { /*ExportFlowOrdersTheme*/ 251, /*Theme =*/ 221 },
                    { /*ExportFlowOrdersThemeBranch*/ 252, /*ThemeOrganizationUnit =*/ 224 },
                    { /*ExportToMsCrmHotClients*/ 256, /*HotClientRequest =*/ 257 },
                    { /*ExportFlowFinancialDataClient*/ 255, /*Client =*/ 200 },
                    { /*ExportFlowPriceListsPriceList*/ 261, /*Price =*/ 155 },
                    { /*ExportFlowPriceListsPriceListPosition*/ 262, /*PricePosition =*/ 154 }
                    // отдаем приоритет обработки всех накопившихся failed orders обработчику ExportFlowOrdersOrder { /*ExportFlowOrdersInvoice*/ 264, /*Order =*/ 151 }
                    // данный обработчик не использует FailedEntities ,{ /*ImportedFirmAddress*/ 253, /*FirmAddress =*/ 164 }
                };

            const string ScriptTemplate = "UPDATE {0} SET [{1}] = {2} WHERE EntityName = {3}";

            var sb = new StringBuilder();
            foreach (var mapEntry in processor2EntitiesMap)
            {
                sb.AppendFormat(ScriptTemplate, ErmTableNames.ExportFailedEntities, TargetColumnName, mapEntry.Key, mapEntry.Value)
                  .Append(Environment.NewLine);
            }

            database.ExecuteNonQuery(sb.ToString());
        }

        private void Complete(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ExportFailedEntities);
            var column = table.Columns[TargetColumnName];
            if (column.DefaultConstraint != null)
            {
                column.DefaultConstraint.Drop();
            }
        }
    }
}
