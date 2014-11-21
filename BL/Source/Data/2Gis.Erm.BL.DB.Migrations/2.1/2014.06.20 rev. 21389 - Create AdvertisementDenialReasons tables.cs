using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21389, "Добавление таблицы DenialReasons", "y.baranihin")]
    public class Migration21389 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetTableDescription = ErmTableNames.DenialReasons;

            var table = context.Database.Tables[targetTableDescription.Name, targetTableDescription.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableDescription.Name, targetTableDescription.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Name", DataType.NVarChar(256), false);
            table.CreateField("Description", DataType.NVarCharMax, true);
            table.CreateField("ProofLink", DataType.NVarChar(256), false);
            table.CreateField("Type", DataType.Int, false);
            table.CreateField("CreatedBy", DataType.BigInt, false);
            table.CreateField("ModifiedBy", DataType.BigInt, true);
            table.CreateField("CreatedOn", DataType.DateTime2(2), false);
            table.CreateField("ModifiedOn", DataType.DateTime2(2), true);
            table.CreateField("IsActive", DataType.Bit, false);
            table.CreateField("Timestamp", DataType.Timestamp, false);

            table.Create();
            table.CreatePrimaryKey("Id");
        }
    }
}