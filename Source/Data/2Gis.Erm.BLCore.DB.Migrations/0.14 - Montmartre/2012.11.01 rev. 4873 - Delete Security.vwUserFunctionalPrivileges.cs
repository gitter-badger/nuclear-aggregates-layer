using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4873, "Удаляем Security.vwUserFunctionalPrivileges")]
    public sealed class Migration4873 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropView1(context);
        }

        private static void DropView1(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserFunctionalPrivileges", ErmSchemas.Security];
            if (view == null)
                return;

            view.Drop();
        }
    }
}