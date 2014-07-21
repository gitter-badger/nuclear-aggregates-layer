using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22367, "Обновляем репликацию договоров", "y.baranihin")]
    public class Migration22367 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var query = string.Format(Properties.Resources.Migration22367, context.CrmDatabaseName);
            context.Connection.ExecuteNonQuery(query);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Erm; }
        }
    }
}