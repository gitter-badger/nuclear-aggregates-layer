using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8736, "Добавлена колонка [RestrictChildPositionPlatforms] в [Billing].[Positions]")]
    public sealed class Migration8736 : TransactedMigration
    {
        private static readonly string SqlStatement = @"
update Billing.Positions
set RestrictChildPositionPlatforms = (select case when count(*) = 0 then 1 else 0 end
		from Billing.Positions as CompositePositions
			inner join Billing.PositionChildren on PositionChildren.MasterPositionId = CompositePositions.Id
			inner join Billing.Positions as ChildPositions on ChildPositions.Id = PositionChildren.ChildPositionId
		where CompositePositions.IsComposite = 1
			and CompositePositions.PlatformId <> ChildPositions.PlatformId
			and CompositePositions.Id = Positions.Id)
where Positions.IsComposite = 1
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Positions);
            if (!table.Columns.Contains("RestrictChildPositionPlatforms"))
            {
                InsertColumn(context.Database, table);
            }

            UpdateValues(context);
        }

        private void UpdateValues(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(SqlStatement);
        }

        private static void InsertColumn(Database database, Table table)
        {
            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(7, 
                        x => 
                        {
                            var c = new Column(x, "RestrictChildPositionPlatforms", DataType.Bit) { Nullable = false };
                            c.AddDefaultConstraint().Text = "0";
                            return c;
                        })
                };
            EntityCopyHelper.CopyAndInsertColumns(database, table, columnsToInsert);
        }
    }
}
