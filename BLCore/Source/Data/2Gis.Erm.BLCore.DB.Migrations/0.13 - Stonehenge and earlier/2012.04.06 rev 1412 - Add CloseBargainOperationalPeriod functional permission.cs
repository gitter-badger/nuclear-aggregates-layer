using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1412, "Создание функционального разрешения \"Закрыть действие договоров\"")]
    // ReSharper disable InconsistentNaming
    public class Migration_1412_CloseBargainOperationalPeriod_Functional_Permission : TransactedMigration
    // ReSharper enable InconsistentNaming
    {
        private const int PrivilegeId = 538;
        private const string PrivilegeName = "PrvCloseBargainOperationalPeriod";
        private const string PrivilegeGrantedName = "FPrvDpthGranted";
        private const int PrivilegeMaskId = 129;

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
