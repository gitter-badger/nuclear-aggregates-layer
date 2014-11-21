using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24270, "Создание представлеиня для ускорения поиска пользователя в MSCRM", "a.tukaev")]
    public class Migration24270 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName != null)
            {
                context.Connection.ExecuteNonQuery(Resources._dbo___SystemUserErmView_24270);
            }
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }
    }
}