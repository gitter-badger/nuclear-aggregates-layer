using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22080, "Выпиливание инфраструктуры для репликации горячих клиентов", "a.tukaev")]
    public class Migration22080 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery("delete from shared.businessoperationservices where service = 10");

            var table = context.Database.GetTable(ErmTableNames.ExportToMsCrmHotClients);
            if (table != null)
            {
                table.Drop();
            }
        }
    }
}