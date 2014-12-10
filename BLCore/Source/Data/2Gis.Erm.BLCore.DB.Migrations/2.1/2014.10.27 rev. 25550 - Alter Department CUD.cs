using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25550, "EF Code First CUD support for Department", "")]
    public class Migration25550 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._Security___DepartmentInsert_25550);
            context.Database.ExecuteNonQuery(Resources._Security___DepartmentUpdate_25550);
        }
    }
}