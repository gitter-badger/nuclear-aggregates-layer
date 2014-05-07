using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(18654, "Добавление таблицы ExportFlowOrders_Invoice", "i.maslennikov")]
    public class Migration18654 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetTableDescription = ErmTableNames.ExportFlowOrdersInvoice;
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
