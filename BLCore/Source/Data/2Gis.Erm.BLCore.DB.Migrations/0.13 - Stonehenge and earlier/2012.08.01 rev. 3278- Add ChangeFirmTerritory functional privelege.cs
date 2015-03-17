using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3278, "Создание функционального разрешения \"Смена территории у фирмы\"")]
    public class Migration3278 : TransactedMigration 
    {
        private const int PrivilegeId = 545;
        private const string PrivilegeName = "PrvChangeFirmTerritory";
        private const string PrivilegeGrantedName = "FPrvDpthGranted";
        private const int PrivilegeMaskId = 136;

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (!PermissionsHelper.CheckPermissionExistence(context, PrivilegeId))
            {
                return;
            }

            var commandQuery = new StringBuilder();

            PermissionsHelper.InsertPrivilege(commandQuery, PrivilegeId, PrivilegeName);
            PermissionsHelper.InsertPrivilegeDepth(commandQuery, PrivilegeId, PrivilegeGrantedName, PrivilegeMaskId);
            context.Connection.ExecuteNonQuery(commandQuery.ToString());
        }
    }
}
