using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201411241430, "Удаление поддержки логирования config файла точки входа в БД Logging", "i.maslennikov")]
    public class Migration201411241430 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
		}
    }
}