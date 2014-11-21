using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4264, "Исправление бага в процедуре ReplicateDeal.")]
    public class Migration4264 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            SchemaQualifiedObjectName replicateDealProc = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateDeal");
            ReplicationHelper.UpdateStoredProcUsingAttachedTemplate(context, this, replicateDealProc, null);
        }
    }
}
