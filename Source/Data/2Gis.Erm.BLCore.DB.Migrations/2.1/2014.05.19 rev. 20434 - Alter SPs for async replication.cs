using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20434, "Обновление хранимых процедур для поддержки рефакторинга асинхронной репликации", "i.maslennikov")]
    public class Migration20434 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // alter SPs
            context.Database.ExecuteNonQuery(Resources._20434_ImportFirmPromising);
            context.Database.ExecuteNonQuery(Resources._20434_UpdateBuildings);
            context.Database.ExecuteNonQuery(Resources._20434_ImportCardsFromXml);
            context.Database.ExecuteNonQuery(Resources._20434_ImportFirmFromXml);
        }
    }
}
