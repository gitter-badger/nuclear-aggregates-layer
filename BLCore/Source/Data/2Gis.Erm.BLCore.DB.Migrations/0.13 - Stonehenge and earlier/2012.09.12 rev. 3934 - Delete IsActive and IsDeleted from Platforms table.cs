using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3934, "Удаляем колонки IsActive и IsDeleted из таблицы Platforms")]
    public sealed class Migration3934 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropIsActiveColumn(context);
            DropIsDeletedColumn(context);
        }

        private static void DropIsActiveColumn(IMigrationContext context)
        {
            const string platforms = "Platforms";
            const string isActive = "IsActive";

            var platformsTable = context.Database.Tables[platforms, ErmSchemas.Billing];
            var isActiveColumn = platformsTable.Columns[isActive];

            if (isActiveColumn == null)
                return;

            isActiveColumn.Drop();
        }

        private static void DropIsDeletedColumn(IMigrationContext context)
        {
            const string platforms = "Platforms";
            const string isDeleted = "IsDeleted";

            var platformsTable = context.Database.Tables[platforms, ErmSchemas.Billing];
            var isActiveColumn = platformsTable.Columns[isDeleted];

            if (isActiveColumn == null)
                return;

            isActiveColumn.Drop();
        }
    }
}
