using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4996, "Удаление хранимой процедуры ReplicateUserTerritory")]
    public sealed class Migration4996 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DroptSp1(context);
        }

        private static void DroptSp1(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ReplicateUserTerritory", ErmSchemas.Security];
            if (sp == null)
                return;

            sp.Drop();
        }
    }
}