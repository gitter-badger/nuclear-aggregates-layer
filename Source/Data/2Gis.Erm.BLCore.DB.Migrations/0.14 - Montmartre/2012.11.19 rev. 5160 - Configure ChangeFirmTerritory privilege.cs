using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5160, "Удаление привелегии ChangeFirmTerritory у всех за исключением администраторов")]
    public sealed class Migration5160 : TransactedMigration
    {
        private const int SystemAdministratorRoleId = 1;
        private const int ChangeFirmTerritoryPrivilegeId = 545;

        private static readonly string DeleteCommand = String.Format(
            @"delete from [Security].[RolePrivileges] " +
            @"where PrivilegeId = {0} and RoleId <> {1}",
            ChangeFirmTerritoryPrivilegeId, SystemAdministratorRoleId);

        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterUserTerritoriesOrganizationUnitsView(context);
        }

        private static void AlterUserTerritoriesOrganizationUnitsView(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(DeleteCommand);
        }
   }
}