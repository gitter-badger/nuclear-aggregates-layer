using DoubleGis.Erm.BL.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(15632, "фикс бага ERM-3150, включаем репликацию клиентов", "y.baranihin")]
    public class Migration15632 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.Migration15632);
        }
    }
}
