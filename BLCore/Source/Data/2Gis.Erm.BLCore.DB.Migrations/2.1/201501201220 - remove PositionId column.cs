using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201501201220, "Удаляем колонку PositionId в Billing.Charges", "y.baranihin")]
    public class Migration201501201220 : TransactedMigration
    {
        private const string TargetColumnName = "PositionId";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.Charges;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(_targetTableName);
            if (!table.Columns.Contains(TargetColumnName))
            {
                return;
            }

            var column = table.Columns[TargetColumnName];
            column.Drop();
        }
    }
}