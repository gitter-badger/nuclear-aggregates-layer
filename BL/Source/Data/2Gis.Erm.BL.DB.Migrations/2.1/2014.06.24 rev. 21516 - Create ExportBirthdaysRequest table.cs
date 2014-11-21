using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(21516, "Добавление таблицы заявок на выгрузку поздравлений именинников", "y.baranihin")]
    public class Migration21516 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateJournalSchema(context.Database);
            CreateTable(context, ErmTableNames.BirthdayCongratulations);
        }

        private static void CreateJournalSchema(Database database)
        {
            if (database.Schemas[ErmSchemas.Journal] != null)
            {
                return;
            }

            var schema = new Schema(database, ErmSchemas.Journal);
            schema.Create();
        }

        private static void CreateTable(IMigrationContext context, SchemaQualifiedObjectName targetTableDescription)
        {
            var table = context.Database.Tables[targetTableDescription.Name, targetTableDescription.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableDescription.Name, targetTableDescription.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("CongratulationDate", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
            table.CreateUniqueIndex("CongratulationDate");
        }
    }
}