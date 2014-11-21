using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4874, "Удаляем колонки IsDeleted в таблицах UserTerritories и UserOrganizationUnits")]
    public sealed class Migration4874 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropColumn1(context);
            DropColumn2(context);
        }

        private static void DropColumn2(IMigrationContext context)
        {
            var table = context.Database.Tables["UserOrganizationUnits", ErmSchemas.Security];
            var column = table.Columns["IsDeleted"];

            if (column == null)
                return;

            context.Database.ExecuteNonQuery("DELETE FROM Security.UserOrganizationUnits WHERE IsDeleted = 1");

            var index = table.Indexes["IX_UserOrganizationUnits_UserId_OrganizationUnitId_IsDeleted"];
            if (index != null)
            {
                index.Drop();
                context.Database.ExecuteNonQuery(@"CREATE NONCLUSTERED INDEX [IX_UserOrganizationUnits_UserId_OrganizationUnitId] ON [Security].[UserOrganizationUnits]([UserId] ASC, [OrganizationUnitId] ASC)");
            }

            column.Drop();
        }

        private static void DropColumn1(IMigrationContext context)
        {
            var table = context.Database.Tables["UserTerritories", ErmSchemas.Security];
            var column = table.Columns["IsDeleted"];

            if (column == null)
                return;

            context.Database.ExecuteNonQuery("DELETE FROM Security.UserTerritories WHERE IsDeleted = 1");
            column.Drop();
        }
    }
}