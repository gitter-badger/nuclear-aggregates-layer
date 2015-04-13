using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504090414, "Исправление часовых поясов MS Dynamics", "a.rechkalov")]
    public class Migration201504090414 : TransactedMigration, INonDefaultDatabaseMigration
    {
        private const string MarkerTimeZone = "Kaliningrad Standard Time";
        private const string MarkerQueryTemplate = "select count (*) from [dbo].[TimeZoneDefinitionBase] where StandardName = '{0}'";

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var count = (int)context.Connection.ExecuteScalar(string.Format(MarkerQueryTemplate, MarkerTimeZone));
            if (count > 0)
            {
                return;
            }
            
            var script = Resources.script_201504090414;
            context.Connection.ExecuteNonQuery(script);
        }
    }
}