using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3904, "Добавление индекса для таблицы ActionsHistoryDetails на колонку PropertyName")]
    public sealed class Migration3904 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string columnName = "PropertyName";
            CreateIndexForTable(context.Database, ErmTableNames.ActionsHistoryDetails, columnName);
        }

        private static void CreateIndexForTable(Database database, SchemaQualifiedObjectName actionsHistoryDetails, string columnName)
        {
            var indexName = string.Format("IX_{0}_{1}", actionsHistoryDetails.Name, columnName);
            var table = database.Tables[actionsHistoryDetails.Name, actionsHistoryDetails.Schema];
            
            var index = table.Indexes[indexName];
            if (index != null)
            {
                return;
            }
            index = new Index(table, indexName) {IsUnique = false, IsClustered = false};
            index.IndexedColumns.Add(new IndexedColumn(index, columnName));
            index.Create();
        }
    }
}
