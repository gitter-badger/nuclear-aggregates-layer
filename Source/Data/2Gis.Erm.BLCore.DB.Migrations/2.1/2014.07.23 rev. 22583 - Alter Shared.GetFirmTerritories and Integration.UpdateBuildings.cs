using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22583, "Обновление Shared.GetFirmTerritories и Integration.UpdateBuildings для поддержания обратной совместимости с flowCards", "a.tukaev")]
    public class Migration22583 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.Migration22583_GetFirmTerritories);
            context.Database.ExecuteNonQuery(Resources.Migration22583_UpdateBuildings);
        }
    }
}
