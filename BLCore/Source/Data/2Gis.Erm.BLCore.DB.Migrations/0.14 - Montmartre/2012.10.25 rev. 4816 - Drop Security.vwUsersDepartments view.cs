using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4816, "Удаляем view Security.vwUsersDepartments, т.к. удален ICompanyHierarchyRepository")]
    public sealed class Migration4816 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var view = context.Database.Views["vwUsersDepartments", ErmSchemas.Security];
            if (view == null)
            {
                return;
            }

            view.Drop();
        }
    }
}
