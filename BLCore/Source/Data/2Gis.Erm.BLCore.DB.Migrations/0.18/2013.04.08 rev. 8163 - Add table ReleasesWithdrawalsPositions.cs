using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8163, "Добавляем таблицу ReleasesWithdrawalsPositions")]
    public sealed class Migration5905 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddThemeTemplatesTable(context);
            RemoveOwnerCodeFromReleasesWithdrawals(context);
            AddTimestampToReleasesWithdrawals(context);
        }

        private void AddThemeTemplatesTable(IMigrationContext context)
        {
            const string commonCreatedByColumnName = "CreatedBy";
            const string commonCreatedOnColumnName = "CreatedOn";
            const string commonModifiedByColumnName = "ModifiedBy";
            const string commonModifiedOnColumnName = "ModifiedOn";

            var table = context.Database.Tables[ErmTableNames.ReleasesWithdrawalsPositions.Name, ErmTableNames.ReleasesWithdrawalsPositions.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.ReleasesWithdrawalsPositions.Name, ErmTableNames.ReleasesWithdrawalsPositions.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("ReleasesWithdrawalId", DataType.BigInt, false);
            table.CreateField("PositionId", DataType.BigInt, false);
            table.CreateField("PlatformId", DataType.BigInt, false);
            table.CreateField("AmountToWithdraw", DataType.Decimal(4, 19), false);
            table.CreateField("Vat", DataType.Decimal(4, 19), false);

            table.Columns.Add(new Column(table, commonCreatedByColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, commonCreatedOnColumnName, DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, commonModifiedByColumnName, DataType.Int) { Nullable = true });
            table.Columns.Add(new Column(table, commonModifiedOnColumnName, DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "Timestamp", DataType.Timestamp) { Nullable = false });

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("ReleasesWithdrawalId", ErmTableNames.ReleasesWithdrawals, "Id");
            table.CreateForeignKey("PositionId", ErmTableNames.Positions, "Id");
            table.CreateForeignKey("PlatformId", ErmTableNames.Platforms, "Id");
        }

        private void RemoveOwnerCodeFromReleasesWithdrawals(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ReleasesWithdrawals.Name, ErmTableNames.ReleasesWithdrawals.Schema];

            var column = table.Columns["OwnerCode"];
            if (column != null)
            {
                column.Drop();
            }
        }

        private void AddTimestampToReleasesWithdrawals(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ReleasesWithdrawals.Name, ErmTableNames.ReleasesWithdrawals.Schema];

            var column = table.Columns["Timestamp"];
            if (column != null)
            {
                return; 
            }

            table.Columns.Add(new Column(table, "Timestamp", DataType.Timestamp) { Nullable = false });
            table.Alter();
        }
    }
}
