using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23100, "Обновляем репликацию сделок", "y.baranihin")]
    public class Migration23100 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Erm; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var query = string.Format(Properties.Resources.Migration23100, context.CrmDatabaseName);
            context.Connection.ExecuteNonQuery(query);
        }
    }
}