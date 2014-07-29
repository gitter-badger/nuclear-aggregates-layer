using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2887, "Добавление таблицы 'Operation'")]
    public class Migration2887 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (!context.Database.Tables.Contains(ErmTableNames.Operations.Name, ErmTableNames.Operations.Schema))
            {
                Table operationsTable = new Table(context.Database, ErmTableNames.Operations.Name, ErmTableNames.Operations.Schema);

                Column idColumn = new Column(operationsTable, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 };
                operationsTable.Columns.Add(idColumn);
                operationsTable.Columns.Add(new Column(operationsTable, "Guid", DataType.UniqueIdentifier) {Nullable = false});
                operationsTable.Columns.Add(new Column(operationsTable, "FileId", DataType.Int));
                operationsTable.Columns.Add(new Column(operationsTable, "Status", DataType.TinyInt) { Nullable = false });
                operationsTable.Columns.Add(new Column(operationsTable, "StartTime", DataType.DateTime2(2)));
                operationsTable.Columns.Add(new Column(operationsTable, "OwnerCode", DataType.Int) {Nullable = false});
                operationsTable.Columns.Add(new Column(operationsTable, "Timestamp", DataType.Timestamp));

                operationsTable.Create();
                operationsTable.CreatePrimaryKey("Id");

                ForeignKey fk = new ForeignKey(operationsTable, "FK_Operations_Files");
                fk.ReferencedTable = ErmTableNames.Files.Name;
                fk.ReferencedTableSchema = ErmTableNames.Files.Schema;

                ForeignKeyColumn fkColumn = new ForeignKeyColumn(fk, "FileId", "Id");
                fk.Columns.Add(fkColumn);
                operationsTable.ForeignKeys.Add(fk);
                fk.Create();
            }
        }
    }
}
