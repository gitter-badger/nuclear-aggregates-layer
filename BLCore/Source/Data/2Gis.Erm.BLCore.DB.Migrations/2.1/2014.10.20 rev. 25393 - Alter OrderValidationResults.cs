using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25393, "Удаляем OrderValidationResults.OrderValidationType", "i.maslennikov")]
    public sealed class Migration25393 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string TargetColumnName = "OrderValidationType";
            if (context.Database.Tables.Contains(ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema))
            {
                var table = context.Database.Tables[ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema];
                if (table.Columns.Contains(TargetColumnName))
                {
                    var column = table.Columns[TargetColumnName];
                    column.Drop();
                }
            }
        }
    }
}
