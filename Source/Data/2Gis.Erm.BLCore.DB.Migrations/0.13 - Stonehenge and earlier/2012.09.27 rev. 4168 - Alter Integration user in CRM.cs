using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4168, "Изменение пользователя Integration в CRM")]
    public class Migration4168 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            string script = string.Format("UPDATE {0} SET DomainName='2GIS\\Integration' WHERE FullName = 'Integration'", CrmTableNames.SystemUserBase);
            context.Connection.ExecuteNonQuery(script);
        }

        public ErmConnectionStringKey ConnectionStringKey { get { return ErmConnectionStringKey.CrmDatabase;}}

    }
}
