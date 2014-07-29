using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21221, "Удаление инфраструктуры асинхронной репликации в MsCRM, основанной на timestamps ([Shared].[ReplicateEntitiesToCrm] и т.п.)", "i.maslennikov")]
    public sealed class Migration21221 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropSPs(context, ErmStoredProcedures.ReplicateEntitiesToCrm);
            DropTables(context, ErmTableNames.CrmReplicationInfo, ErmTableNames.CrmReplicationDetails);
        }

        private void DropSPs(IMigrationContext context, params SchemaQualifiedObjectName[] sps)
        {
            foreach (var targetSp in sps)
            {
                var sp = context.Database.StoredProcedures[targetSp.Name, targetSp.Schema];
                if (sp != null)
                {
                    sp.Drop();
                    return;
                }
            }

            context.Database.Alter();
        }

        private void DropTables(IMigrationContext context, params SchemaQualifiedObjectName[] tables)
        {
            var targetTables = 
                tables.Select(t => context.Database.Tables[t.Name, t.Schema])
                      .Where(t => t != null)
                      .ToArray();

            foreach (var table in targetTables)
            {
                table.Drop();
            }

            context.Database.Alter();
        }
    }
}
