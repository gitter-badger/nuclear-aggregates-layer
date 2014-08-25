using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23425, "a.tukaev", "Обновление активных территорий у клиентов при импорте фирм из IR (fix ERM-4772)")]
    public class Migration23425 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.ImportFirmFromXml_23425);
        }
    }
}