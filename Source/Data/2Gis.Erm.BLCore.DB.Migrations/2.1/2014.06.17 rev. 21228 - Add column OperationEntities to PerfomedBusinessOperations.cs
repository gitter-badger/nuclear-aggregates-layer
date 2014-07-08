using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21228, "Добавление колонки OperationEntities в таблицу [Shared].[PerformedBusinessOperations]", "i.maslennikov")]
    public sealed class Migration21228 : TransactedMigration
    {
        private const string TargetColumnName = "OperationEntities";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.PerformedBusinessOperations;
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(_targetTableName);
            if (table.Columns.Contains(TargetColumnName))
            {
                return;
            }

            InsertColumn(table);
        }

        private void InsertColumn(Table table)
        {
            var column = ColumnCreator(table);
            table.Columns.Add(column);
            table.Alter();
        }

        private Column ColumnCreator(SqlSmoObject smo)
        {
            return new Column(smo, TargetColumnName, DataType.NVarChar(1024)) { Nullable = true };
        }
    }
}
