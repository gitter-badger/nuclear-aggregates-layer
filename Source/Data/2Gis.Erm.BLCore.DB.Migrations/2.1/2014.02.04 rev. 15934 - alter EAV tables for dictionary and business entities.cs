using System.Data;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(15934, "Обновление схемы EAV таблицы для справочных и бизнес-сущностей")]
    public class Migration15934 : TransactedMigration
    {
        private const string ReferencedEntityIdColumnName = "ReferencedEntityId";

        private const string EntityIdColumnName = "EntityId";
        private const string EntityIntanceIdColumnName = "EntityInstanceId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddEntityIdColumnToEntityIntancesTable(context.Database, ErmTableNames.DictionaryEntityInstances);
            RenameEntityIdColumnInEntityPropertyIntancesTable(context.Database, ErmTableNames.DictionaryEntityPropertyInstances);

            AddEntityIdColumnToEntityIntancesTable(context.Database, ErmTableNames.BusinessEntityInstances);
            RenameEntityIdColumnInEntityPropertyIntancesTable(context.Database, ErmTableNames.BusinessEntityPropertyInstances);
        }

        private static void AddEntityIdColumnToEntityIntancesTable(Database database, SchemaQualifiedObjectName tableName)
        {
            var table = database.Tables[tableName.Name, tableName.Schema];
            if (table.Columns.Contains(ReferencedEntityIdColumnName))
            {
                return;
            }

            table.CreateField(EntityIdColumnName, DataType.BigInt, true);
            table.Alter();
        }

        private static void RenameEntityIdColumnInEntityPropertyIntancesTable(Database database, SchemaQualifiedObjectName tableName)
        {
            var table = database.Tables[tableName.Name, tableName.Schema];
            var column = table.Columns[EntityIdColumnName];
            if (column == null)
            {
                return;
            }

            var indexesTable = column.EnumIndexes();
            foreach (DataRow tableRow in indexesTable.Rows)
            {
                var indexName = (string)tableRow["Urn"];
                var index = table.Indexes[indexName];
                index.Drop();
            }

            column.Rename(EntityIntanceIdColumnName);
            column.Alter();

            table.CreateIndex(false, EntityIntanceIdColumnName);
            table.Alter();
        }
    }
}
