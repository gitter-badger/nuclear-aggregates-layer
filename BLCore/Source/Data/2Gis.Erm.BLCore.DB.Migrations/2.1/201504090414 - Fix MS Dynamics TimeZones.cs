using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504090414, "Исправление часовых поясов MS Dynamics", "a.rechkalov")]
    public class Migration201504090414 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var script = Resources.script_201504090414;
            context.Database.ExecuteNonQuery(script);
        }
    }
}