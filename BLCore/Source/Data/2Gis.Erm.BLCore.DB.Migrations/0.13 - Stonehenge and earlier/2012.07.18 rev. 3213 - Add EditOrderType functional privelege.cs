using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3213, "Создание функционального разрешения \"Смена типа заказа\"")]
    public class Migration3213 : TransactedMigration
    {
        private const int PrivilegeId = 544;
        private const string PrivilegeName = "PrvEditOrderType";
        private const string PrivilegeGrantedName = "FPrvDpthGranted";
        private const int PrivilegeMaskId = 135;

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
