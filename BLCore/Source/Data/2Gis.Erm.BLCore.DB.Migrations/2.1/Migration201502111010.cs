using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502111010, "Удаляем колонку Timestamp", "y.baranihin")]
    public class Migration201502111010 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.SalesModelCategoryRestrictions.Name, ErmTableNames.SalesModelCategoryRestrictions.Schema];
            var columnn = table.Columns["Timestamp"];
            if (columnn == null)
            {
                return;
            }

            columnn.Drop(); 
        }
    }
}