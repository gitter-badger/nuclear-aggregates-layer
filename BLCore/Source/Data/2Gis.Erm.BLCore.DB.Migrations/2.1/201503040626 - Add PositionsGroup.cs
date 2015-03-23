using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201503040626, "Добавляем атрибут группа номенклатур", "y.baranihin")]
    public class Migration201503040626 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var positions = context.Database.GetTable(ErmTableNames.Positions);
            const string GroupColumn = "PositionsGroup";
            const int UndefinedPositionGroup = 0;

            var newColumns = new[]
                                 {
                                     new InsertedColumnDefinition(11, x => new Column(x, GroupColumn, DataType.Int) { Nullable = true }),
                                 };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, positions, newColumns);

            context.Connection.ExecuteNonQuery(string.Format("Update [{0}].[{1}] set [{2}]={3}", positions.Schema, positions.Name, GroupColumn, UndefinedPositionGroup));
            positions = context.Database.GetTable(ErmTableNames.Positions);
            positions.Columns[GroupColumn].Nullable = false;
            positions.Columns[GroupColumn].Alter();
        }
    }
}