using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(20638, "Добавление ChargeSessionId в Billing.LockDetails", "a.tukaev")]
    public class Migration20638 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var lockDetails = context.Database.GetTable(ErmTableNames.LockDetails);
            const string ChargeSessionId = "ChargeSessionId";

            if (lockDetails == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(4, x => new Column(x, ChargeSessionId, DataType.UniqueIdentifier) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, lockDetails, newColumns);

            lockDetails = context.Database.GetTable(ErmTableNames.LockDetails);
            var idx = new Index(lockDetails, "IX_LockDetails_ChargeSessionId");
            idx.IndexedColumns.Add(new IndexedColumn(idx, ChargeSessionId));
            idx.Create();
        }
    }
}