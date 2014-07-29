using System.Collections.Generic;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4935, "Добавлены колонки в Shared.Operations")]
    public sealed class Migration4935 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Operations);

            if(!table.Columns.Contains("FinishTime"))
            {
                List<InsertedColumnDefinition> columnsToInsert = new List<InsertedColumnDefinition>();
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "FinishTime", DataType.DateTime2(2)) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "Description", DataType.NVarCharMax) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "Type", DataType.SmallInt) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "OrganizationUnitId", DataType.Int) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "OwnerCode", DataType.Int) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "CreatedBy", DataType.Int) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "ModifiedBy", DataType.Int) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "CreatedOn", DataType.DateTime2(2)) { Nullable = true }));
                columnsToInsert.Add(new InsertedColumnDefinition(5, x => new Column(x, "ModifiedOn", DataType.DateTime2(2)) { Nullable = true }));

                var alteredTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);

                const string updateOrdersQuery =
@"
GO

UPDATE Shared.Operations SET
Type = 0,
OwnerCode = 1,
CreatedBy = 1,
CreatedOn = StartTime
";

                context.Connection.ExecuteNonQuery(updateOrdersQuery);

                ForeignKey fk = new ForeignKey(alteredTable, "FK_Operations_OrganizationUnits");
                fk.ReferencedTable = ErmTableNames.OrganizationUnits.Name;
                fk.ReferencedTableSchema = ErmTableNames.OrganizationUnits.Schema;
                fk.Columns.Add(new ForeignKeyColumn(fk, "OrganizationUnitId", "Id"));

                alteredTable.ForeignKeys.Add(fk);
                fk.Create();

                // После заливки данных можем развешивать NOT Null.
                alteredTable.SetNonNullableColumns("Type", "OwnerCode", "CreatedBy", "CreatedOn");
                alteredTable.Alter();

                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.AppendLine(@"GO");

                foreach (var i in new [] { 1, 2, 32, 65536 })
                {
                    queryBuilder.AppendFormat("INSERT INTO Security.Privileges (EntityType, Operation) VALUES (217, {0})", i);
                    queryBuilder.AppendLine();
                }

                queryBuilder.AppendLine(@"GO");

                queryBuilder.AppendLine(@"
INSERT INTO Security.RolePrivileges (RoleId, PrivilegeId, Priority, Mask, CreatedBy, CreatedOn)
SELECT r.Id as RoleId, p.Id, 4, 16, 1, GETDATE()
FROM Security.Privileges p
FULL OUTER JOIN Security.Roles r ON 1=1
WHERE p.EntityType = 217 AND p.Operation IN (1) AND r.Id <> 8

GO
");

                context.Connection.ExecuteNonQuery(queryBuilder.ToString());
            }
        }
    }
}
