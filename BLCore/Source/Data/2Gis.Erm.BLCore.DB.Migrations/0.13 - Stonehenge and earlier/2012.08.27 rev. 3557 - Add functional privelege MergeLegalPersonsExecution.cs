using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3557, "Создание функционального разрешения \"Слияние юридических лиц\"")]
    public class Migration3557 : TransactedMigration
    {
        private const int PrivilegeId = 546;
        private const string PrivilegeName = "PrvMergeLegalPersons";
        private const string PrivilegeGrantedName = "FPrvDpthGranted";
        private const int PrivilegeMaskId = 134;

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
