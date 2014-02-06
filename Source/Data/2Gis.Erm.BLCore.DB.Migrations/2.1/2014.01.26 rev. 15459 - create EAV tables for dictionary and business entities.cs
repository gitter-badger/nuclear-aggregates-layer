using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(15459, "Создаем EAV таблицы для справочных и бизнес-сущностей")]
    public class Migration15459 : TransactedMigration
    {
        private const string IdColumnName = "Id";

        private const string EntityIdColumnName = "EntityId";
        private const string PropertyIdColumnName = "PropertyId";
        private const string TextValueColumnName = "TextValue";
        private const string NumericValueColumnName = "NumericValue";
        private const string DateTimeValueColumnName = "DateTimeValue";

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateDynamicStorageSchema(context.Database);

            CreateEntityIntancesTable(context.Database, ErmTableNames.DictionaryEntityInstances);
            CreateEntityPropertyIntancesTable(context.Database, ErmTableNames.DictionaryEntityPropertyInstances, ErmTableNames.DictionaryEntityInstances);

            CreateEntityIntancesTable(context.Database, ErmTableNames.BusinessEntityInstances);
            CreateEntityPropertyIntancesTable(context.Database, ErmTableNames.BusinessEntityPropertyInstances, ErmTableNames.BusinessEntityInstances);
        }

        private static void CreateDynamicStorageSchema(Database database)
        {
            if (database.Schemas[ErmSchemas.DynamicStorage] != null)
            {
                return;
            }

            var schema = new Schema(database, ErmSchemas.DynamicStorage);
            schema.Create();
        }

        private static void CreateEntityIntancesTable(Database database, SchemaQualifiedObjectName tableName)
        {
            if (database.Tables[tableName.Name, tableName.Schema] != null)
            {
                return;
            }

            var table = new Table(database, tableName.Name, tableName.Schema);

            table.CreateField(IdColumnName, DataType.BigInt, false);
            table.CreateSecureEntityStandartColumns();
            table.Create();

            table.CreatePrimaryKey();
        }

        private static void CreateEntityPropertyIntancesTable(Database database, SchemaQualifiedObjectName tableName, SchemaQualifiedObjectName relatedTableName)
        {
            if (database.Tables[tableName.Name, tableName.Schema] != null)
            {
                return;
            }

            var table = new Table(database, tableName.Name, tableName.Schema);

            table.CreateField(IdColumnName, DataType.BigInt, false);
            table.CreateField(EntityIdColumnName, DataType.BigInt, false);
            table.CreateField(PropertyIdColumnName, DataType.Int, false);
            table.CreateField(TextValueColumnName, DataType.NVarCharMax, true);
            table.CreateField(NumericValueColumnName, DataType.Decimal(2, 9), true);
            table.CreateField(DateTimeValueColumnName, DataType.DateTime2(2), true);
            table.Create();

            table.CreatePrimaryKey();
            table.CreateForeignKey(EntityIdColumnName, relatedTableName, IdColumnName);
            table.CreateIndex(EntityIdColumnName);
        }
    }
}
