using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20866, "Добавление колонки UseCaseId в таблицу [Shared].[PerformedBusinessOperations]", "i.maslennikov")]
    public sealed class Migration20866 : TransactedMigration
    {
        private const string TargetColumnName = "UseCaseId";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.PerformedBusinessOperations;
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 1200;

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
            const string TargetColumnDefaultConstraintName = "DF_" + TargetColumnName;

            var column = new Column(smo, TargetColumnName, DataType.UniqueIdentifier) { Nullable = false };
            column.AddDefaultConstraint(TargetColumnDefaultConstraintName).Text = "CAST('00000000-0000-0000-0000-000000000000' as UNIQUEIDENTIFIER)";
            return column;
        }
    }
}
