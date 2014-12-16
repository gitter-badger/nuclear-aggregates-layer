using System;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Resources;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.MW;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(24480, "Replicates the migrated activities back to MS CRM.", "s.pomadin")]
    public sealed class ReplicateActivitiesMigration : IContextedMigration<IActivityMigrationContext>
    {
        public void Apply(IActivityMigrationContext context)
        {
            context.Connection.StatementTimeout = 1 * 60 * 60; // a hour

            foreach (var tuple in new[]
                {
                    Tuple.Create(Scripts.BulkAppointmentReplication, context.CrmDatabaseName, "appointments"),
                    Tuple.Create(Scripts.BulkLetterReplication, context.CrmDatabaseName, "letters"),
                    Tuple.Create(Scripts.BulkTaskReplication, context.CrmDatabaseName, "tasks"),
                    Tuple.Create(Scripts.BulkPhonecallReplication, context.CrmDatabaseName, "phonecalls"),
                })
            {
                try
                {
                    context.Connection.BeginTransaction();
                    context.Connection.ExecuteNonQuery(BuildSql(tuple.Item1, tuple.Item2));
                    context.Connection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Replication of {0} was failed.", tuple.Item3);
                    Console.WriteLine(ex);
                    context.Connection.RollBackTransaction();
                    throw;
                }
            }
        }

        public void Revert(IActivityMigrationContext context)
        {
            throw new NotImplementedException();
        }

        private static string BuildSql(string script, string crmDatabaseName)
        {
            return string.Format(script, crmDatabaseName);
        }
    }
}
