using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8192, "Инфраструктура действий в базе ERM")]
    public sealed class Migration8192 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            if (context.Database.Schemas["Activity"] == null)
            {
                var activitySchema = new Schema(context.Database, "Activity");
                activitySchema.Create();
            }

            if (context.Database.GetTable(ErmTableNames.ActivityInstances) == null)
            {
                var newTable = context.Database.CreateTable(ErmTableNames.ActivityInstances);

                newTable.Columns.Add(new Column(newTable, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentityIncrement = 1, IdentitySeed = 1 });
                newTable.CreateField("Type", DataType.Int, false);
                newTable.CreateField("ClientId", DataType.BigInt, true);
                newTable.CreateField("ContactId", DataType.BigInt, true);
                newTable.CreateField("DealId", DataType.BigInt, true);
                newTable.CreateField("FirmId", DataType.BigInt, true);
                newTable.CreateSecureEntityStandartColumns();
                newTable.Create();
                newTable.CreatePrimaryKey();

                newTable.CreateForeignKey("ClientId", ErmTableNames.Clients, "Id");
                newTable.CreateForeignKey("DealId", ErmTableNames.Deals, "Id");
                newTable.CreateForeignKey("FirmId", ErmTableNames.Firms, "Id");
                newTable.CreateForeignKey("ContactId", ErmTableNames.Contacts, "Id");
            }

            if (context.Database.GetTable(ErmTableNames.ActivityPropertyInstances) == null)
            {
                var newTable = context.Database.CreateTable(ErmTableNames.ActivityPropertyInstances);

                newTable.Columns.Add(new Column(newTable, "Id", DataType.BigInt)
                {
                    Nullable = false,
                    Identity = true,
                    IdentityIncrement = 1,
                    IdentitySeed = 1
                });

                newTable.Columns.Add(new Column(newTable, "ActivityId", DataType.BigInt) { Nullable = false });
                newTable.CreateField("PropertyId", DataType.Int, false);
                newTable.Columns.Add(new Column(newTable, "TextValue", DataType.NVarCharMax) { Nullable = true });
                newTable.Columns.Add(new Column(newTable, "NumericValue", DataType.Decimal(2, 9)) { Nullable = true });
                newTable.Columns.Add(new Column(newTable, "DateTimeValue", DataType.DateTime2(2)) { Nullable = true });

                newTable.Create();
                newTable.CreatePrimaryKey();

                var index = new Index(newTable, "IX_ActivityExtensions_ActivityId");
                index.IndexedColumns.Add(new IndexedColumn(index, "ActivityId"));
                newTable.Indexes.Add(index);
                index.Create();

                newTable.CreateForeignKey("ActivityId", ErmTableNames.ActivityInstances, "Id");
            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
