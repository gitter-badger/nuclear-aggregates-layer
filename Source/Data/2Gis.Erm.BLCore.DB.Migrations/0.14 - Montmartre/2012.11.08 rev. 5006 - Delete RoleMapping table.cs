using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5006, "Удаление таблицы RoleMappings")]
    public sealed class Migration5006 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DroptTable1(context);
        }

        private static void DroptTable1(IMigrationContext context)
        {
            var table = context.Database.Tables["RoleMappings", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }
    }
}