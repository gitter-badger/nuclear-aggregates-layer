using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4855, "Удаляем Security.EntityPrivilegeDepths")]
    public sealed class Migration4855 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["EntityPrivilegeDepths", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }
    }
}