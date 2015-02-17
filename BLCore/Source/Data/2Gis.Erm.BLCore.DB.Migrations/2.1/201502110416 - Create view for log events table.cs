 using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
 using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502110416, "Создаем View для прсмотра Log.Events в локальном времени", "i.maslennikov")]
    public class Migration201502110416 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Logging; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.Views.Contains("Events_LocalTimeZone", "Log"))
            {
                return;
            }

            context.Database.ExecuteNonQuery(Resources._201502110416_Log_Events_DateLocalTimeZone);
        }
    }
}
