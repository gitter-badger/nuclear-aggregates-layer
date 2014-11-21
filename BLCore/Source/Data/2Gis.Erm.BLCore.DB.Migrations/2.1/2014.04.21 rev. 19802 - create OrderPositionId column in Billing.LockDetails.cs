using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19802, "Добавление OrderPositionId в Billing.LockDetails", "a.tukaev")]
    public class Migration19802 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var lockDetails = context.Database.GetTable(ErmTableNames.LockDetails);
            const string OrderPositionId = "OrderPositionId";

            if (lockDetails == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(3, x => new Column(x, OrderPositionId, DataType.BigInt) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, lockDetails, newColumns);

            lockDetails = context.Database.GetTable(ErmTableNames.LockDetails);
            var idx = new Index(lockDetails, "IX_LockDetails_OrderPositionId");
            idx.IndexedColumns.Add(new IndexedColumn(idx, OrderPositionId));
            idx.Create();
        }
    }
}