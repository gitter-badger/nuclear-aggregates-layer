using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5224, "Избавляемся от нулевых значений в колонке HasDocumentsDebt в заказах и договорах.")]
    public sealed class Migration5224 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(String.Format("UPDATE {0} SET HasDocumentsDebt = 2 WHERE HasDocumentsDebt = 0", ErmTableNames.Orders));
            context.Connection.ExecuteNonQuery(String.Format("UPDATE {0} SET HasDocumentsDebt = 2 WHERE HasDocumentsDebt = 0", ErmTableNames.Bargains));
        }
    }
}
