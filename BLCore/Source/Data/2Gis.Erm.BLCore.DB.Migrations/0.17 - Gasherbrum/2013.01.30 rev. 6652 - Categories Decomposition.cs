using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6652, "Декомпозиция цен по рубрикам фирмы.")]
    public sealed class Migration6652 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            if (context.Database.GetTable(ErmTableNames.CategoryGroups) == null)
            {
                var newTable = new Table(context.Database, ErmTableNames.CategoryGroups.Name, ErmTableNames.CategoryGroups.Schema);
                newTable.Columns.Add(new Column(newTable, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
                newTable.CreateField("CategoryGroupName", DataType.NVarChar(256), false);
                newTable.CreateField("GroupRate", DataType.Decimal(4, 19), false);
                ErmTableUtilsForOldIntKeys.CreateSecureEntityStandartColumns(newTable);
                newTable.Create();
                newTable.CreatePrimaryKey();

                string insertEntityPrivilegesCommandTemplate = @"INSERT INTO Security.Privileges (EntityType, Operation) VALUES ({0}, 1), ({0}, 2), ({0}, 32), ({0}, 65536)";
                int categoryGroupId = 162;

                var command = string.Format(insertEntityPrivilegesCommandTemplate, categoryGroupId);
                context.Connection.ExecuteNonQuery(command);
            }

            var table = context.Database.GetTable(ErmTableNames.CategoryOrganizationUnits);

            if (!table.Columns.Contains("CategoryGroupId"))
            {
                var columnsToInsert = new[] { new InsertedColumnDefinition(3, x => new Column(x, "CategoryGroupId", DataType.Int) { Nullable = true }) };
                table = EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
                table.CreateForeignKey("CategoryGroupId", ErmTableNames.CategoryGroups, "Id");
                table.Alter();
            }

            table = context.Database.GetTable(ErmTableNames.PricePositions);

            if (!table.Columns.Contains("RatePricePositions"))
            {
                var columnToInsert = new InsertedColumnDefinition(9, x => new Column(x, "RatePricePositions", DataType.Bit) { Nullable = true });
                table.InsertAndSetNonNullableColumn(context, columnToInsert, "RatePricePositions", "0");
            }

            table = context.Database.GetTable(ErmTableNames.CategoryFirmAddresses);

            if (!table.Columns.Contains("IsPrimary"))
            {
                var columnToInsert = new InsertedColumnDefinition(4, x => new Column(x, "IsPrimary", DataType.Bit) { Nullable = true });
                table.InsertAndSetNonNullableColumn(context, columnToInsert, "IsPrimary", "0");
            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
