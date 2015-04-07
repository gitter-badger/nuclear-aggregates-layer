using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201501240204, "Переименовываем колонку BillNumber", "y.baranihin")]
    public class Migration201501240204 : TransactedMigration
    {
        private const string BillNumberColumn = "BillNumber";
        private const string NumberColumn = "Number";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var billsTable = context.Database.Tables[ErmTableNames.Bills.Name, ErmTableNames.Bills.Schema];
            billsTable.Columns[BillNumberColumn].Rename(NumberColumn);
        }
    }
}