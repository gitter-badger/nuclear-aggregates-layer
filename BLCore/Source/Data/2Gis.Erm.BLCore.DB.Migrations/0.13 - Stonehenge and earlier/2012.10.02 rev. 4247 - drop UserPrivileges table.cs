using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4247, "Удаление таблицы UserPrivileges")]
    public sealed class Migration4247 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DeleteUserPrivilegesTable(context);
            AlterView(context);
        }

        private static void AlterView(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserFunctionalPrivileges", ErmSchemas.Security];

            view.TextBody =
            @"SELECT ROW_NUMBER() OVER (ORDER BY UserId, PrivilegeId) AS Id, U.Id as UserId, P.Id as PrivilegeId, RP.Priority, RP.Mask FROM Security.Users U
            INNER JOIN Security.UserRoles UR ON UR.UserId = U.Id
            INNER JOIN Security.RolePrivileges RP ON RP.RoleId = UR.RoleId
            INNER JOIN Security.Privileges P ON P.Id = RP.PrivilegeId
            WHERE P.PrivilegeType = N'F' AND P.IsDeleted = 0 AND P.IsActive = 1";

            view.Alter();
        }

        private static void DeleteUserPrivilegesTable(IMigrationContext context)
        {
            var table = context.Database.Tables["UserPrivileges", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }
    }
}
