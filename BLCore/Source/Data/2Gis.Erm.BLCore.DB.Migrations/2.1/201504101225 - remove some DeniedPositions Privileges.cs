using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101225, "Удаляем часть сущностных привелегий для запрещенных позиций", "y.baranihin")]
    public class Migration201504101225 : TransactedMigration
    {
        private const int DeniedPositionEntityCode = 180;
        private const int Append = 4;
        private const int AppendTo = 16;
        private const int Share = 262144;
        private const int Assign = 524288;
        private const string RemoveTemplate = @"Declare @PrivilegeId bigint = (select top 1 Id from [Security].[Privileges] where EntityType = {0} and Operation = {1})
                                                Delete from [Security].[RolePrivileges] where PrivilegeId = @PrivilegeId
                                                Delete from [Security].[Privileges] where Id = @PrivilegeId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(RemoveTemplate, DeniedPositionEntityCode, Append));
            context.Connection.ExecuteNonQuery(string.Format(RemoveTemplate, DeniedPositionEntityCode, AppendTo));
            context.Connection.ExecuteNonQuery(string.Format(RemoveTemplate, DeniedPositionEntityCode, Share));
            context.Connection.ExecuteNonQuery(string.Format(RemoveTemplate, DeniedPositionEntityCode, Assign));
        }
    }
}