using System;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Resources;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    /// <summary>
    /// The migration is designed to control the transaction in the SQL script due to performance reasons.
    /// </summary>
    [Migration(23613, "Cleaning up the activities in MS CRM.", "s.pomadin")]
    public sealed class CrmActivitiesCleanup : IContextedMigration<IMigrationContext>, INonDefaultDatabaseMigration
    {
        public void Apply(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 5 * 60 * 60; // 5 hours

            try
            {
                context.Connection.ExecuteNonQuery(Scripts.ActivityCleanup);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cleaning up activity was failed.");
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Revert(IMigrationContext context)
        {
            throw new NotImplementedException();
        }

        public ErmConnectionStringKey ConnectionStringKey { get { return ErmConnectionStringKey.CrmDatabase; } }
    }
}
