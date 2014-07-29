using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5079, "Сихронизаниция индексов для таблиц ActionsHistory и ActionsHistoryDetails")]
    public sealed class Migration5079 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DeleteExistingIndexes(context.Database, ErmTableNames.ActionsHistoryDetails, "IX_ActionsHistoryDetails_PropertyName");
            CreateIndexForTable(context.Database, ErmTableNames.ActionsHistory, "EntityId", "EntityType");
            CreateIndexForTable(context.Database, ErmTableNames.ActionsHistoryDetails, "ActionsHistoryId", "PropertyName");
        }

        private static void DeleteExistingIndexes(Database database, SchemaQualifiedObjectName schemaQualifiedTableName, string indexName)
        {
            var table = database.Tables[schemaQualifiedTableName.Name, schemaQualifiedTableName.Schema];

            var index = table.Indexes[indexName];
            if (index == null)
            {
                return;
            }
            index.Drop();
        }

        private static void CreateIndexForTable(Database database, SchemaQualifiedObjectName schemaQualifiedTableName, string column1Name, string column2Name)
        {
            var indexName = string.Format("IX_{0}_{1}-{2}", schemaQualifiedTableName.Name, column1Name, column2Name);
            var table = database.Tables[schemaQualifiedTableName.Name, schemaQualifiedTableName.Schema];

            var index = table.Indexes[indexName];
            if (index != null)
            {
                return;
            }
            index = new Index(table, indexName) { IsUnique = false, IsClustered = false };
            index.IndexedColumns.Add(new IndexedColumn(index, column1Name));
            index.IndexedColumns.Add(new IndexedColumn(index, column2Name));
            index.Create();
        }
    }
}