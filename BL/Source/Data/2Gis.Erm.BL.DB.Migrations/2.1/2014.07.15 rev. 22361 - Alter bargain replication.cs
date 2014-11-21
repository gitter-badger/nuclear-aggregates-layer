using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22361, "Обновляем репликацию договоров", "y.baranihin")]
    public class Migration22361 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var query = string.Format(Properties.Resources.Migration22361, context.CrmDatabaseName);
            context.Connection.ExecuteNonQuery(query);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Erm; }
        }
    }
}