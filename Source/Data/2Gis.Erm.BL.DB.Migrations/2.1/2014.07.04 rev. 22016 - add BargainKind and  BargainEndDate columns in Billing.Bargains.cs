using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22016, "Добавление колонок BargainKind и BargainEndDate в Billing.Bargains", "y.baranihin")]
    public class Migration22016 : TransactedMigration
    {
        private const int ClientBargainType = 2;
        protected override void ApplyOverride(IMigrationContext context)
        {
            var bargains = context.Database.GetTable(ErmTableNames.Bargains);
            const string BargainKind = "BargainKind";
            const string BargainEndDate = "BargainEndDate";

            if (bargains == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(7, x => new Column(x, BargainEndDate, DataType.DateTime2(2)) { Nullable = true }),
                    new InsertedColumnDefinition(7, x => new Column(x, BargainKind, DataType.Int) { Nullable = true }),
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, bargains, newColumns);

            context.Connection.ExecuteNonQuery("update [Billing].[Bargains] set BargainKind = " + ClientBargainType);

            bargains = context.Database.GetTable(ErmTableNames.Bargains);

            var kindColumn = bargains.Columns["BargainKind"];
            kindColumn.Nullable = false;
            kindColumn.Alter();
        }
    }
}