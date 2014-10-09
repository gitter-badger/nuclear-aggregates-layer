using System;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Resources;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(24480, "Replicates the migrated activities back to MS CRM.", "s.pomadin")]
    public sealed class ReplicateActivitiesMigration : IContextedMigration<IMigrationContext>
    {
        public void Apply(IMigrationContext context)
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
                    context.Connection.RollBackTransaction();
                    Console.WriteLine("Replication of {0} was failed.", tuple.Item3);
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public void Revert(IMigrationContext context)
        {
            throw new NotImplementedException();
        }

        private static string BuildSql(string script, string crmDatabaseName)
        {
            return string.Format(script, crmDatabaseName);
        }
    }
}
