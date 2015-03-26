using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502130238, "Добавляем способ учета оказания услуг в журнал списаний", "y.baranihin")]
    public class Migration201502130238 : TransactedMigration
    {
        private const int UndefinedAccountingMethod = 0;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.WithdrawalInfos);
            const string AccountingMethodColumn = "AccountingMethod";

            var newColumns = new[]
                                 {
                                     new InsertedColumnDefinition(8, x => new Column(x, AccountingMethodColumn, DataType.Int) { Nullable = true })
                                 };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);

            context.Connection.ExecuteNonQuery("update [Billing].[WithdrawalInfos] set AccountingMethod = " + UndefinedAccountingMethod);

            table = context.Database.GetTable(ErmTableNames.WithdrawalInfos);

            var kindColumn = table.Columns[AccountingMethodColumn];
            kindColumn.Nullable = false;
            kindColumn.Alter();
        }
    }
}