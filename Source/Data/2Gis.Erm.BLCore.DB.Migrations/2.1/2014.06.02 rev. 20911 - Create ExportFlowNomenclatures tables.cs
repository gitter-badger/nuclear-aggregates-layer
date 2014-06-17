using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20911, "Добавление таблиц для потока FlowNomenclatures", "a.tukaev")]
    public class Migration20911 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateExportTable(context, ErmTableNames.ExportFlowNomenclaturesNomenclatureElement);
            CreateExportTable(context, ErmTableNames.ExportFlowNomenclaturesNomenclatureElementRelation);
        }

        private static void CreateExportTable(IMigrationContext context, SchemaQualifiedObjectName targetTableDescription)
        {
            var table = context.Database.Tables[targetTableDescription.Name, targetTableDescription.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableDescription.Name, targetTableDescription.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
        }
    }
}