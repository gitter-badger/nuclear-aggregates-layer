using System;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Resources;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    //[Migration(24480, "Replicates the migrated activities back to MS CRM.", "s.pomadin")]
    public sealed class ReplicateActivitiesMigration : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
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
                    context.Connection.ExecuteNonQuery(BuildSql(tuple.Item1, tuple.Item2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Replication of {0} was failed.", tuple.Item3);
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        private static string BuildSql(string script, string crmDatabaseName)
        {
            return string.Format(script, crmDatabaseName);
        }
    }
}
