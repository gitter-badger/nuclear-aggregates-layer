using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1_3
{
    [Migration(13962, "Выдаём привилегию HotClientProcessing роли 'Директор'", "a.rechkalov")]
    public class Migration13962 : TransactedMigration
    {
        private const string SelectCount = "SELECT count(*) FROM [Security].[RolePrivileges] where RoleId = 2 and PrivilegeId = 230714158147869960";

        private const string InsertTemplate = "insert into [Security].[RolePrivileges](Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn) " +
                                              "values (240910321345101825, 2, 230714158147869960, 0, 134, 1, 1, '{0}', '{0}')";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var permissionGranted = (int)context.Connection.ExecuteScalar(SelectCount);
            if (permissionGranted != 0)
            {
                return;
            }

            var insert = string.Format(InsertTemplate, DateTime.UtcNow.ToString("u"));
            context.Connection.ExecuteNonQuery(insert);
        }
    }
}
