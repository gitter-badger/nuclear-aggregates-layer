using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2121, "Создание функционального разрешения \"Оставлять клиента без фирм\"")]
    public class Migration2121 : TransactedMigration 
    {
        private const int PrivilegeId = 542;
        private const string PrivilegeName = "PrvLeaveClientWithNoFirms";
        private const string PrivilegeGrantedName = "FPrvDpthGranted";
        private const int PrivilegeMaskId = 133;

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
