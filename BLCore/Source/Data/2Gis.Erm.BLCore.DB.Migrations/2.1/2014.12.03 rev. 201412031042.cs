using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412031042, "[ERM-5371]: удаление функциональной привелегии 'Формирование ППС'", "s.pomadin")]
    public class Migration201412031042 : TransactedMigration
    {
        private const int PrivilegeId = 605;

        private const string QueryTemplate = 
            @"DELETE FROM [Security].[RolePrivileges] WHERE PrivilegeId in (SELECT Id FROM [Security].[Privileges] WHERE Operation = {0})"
            +"DELETE FROM [Security].[FunctionalPrivilegeDepths] WHERE PrivilegeId IN (SELECT Id FROM [Security].[Privileges] WHERE Operation = {0})"
            +"DELETE FROM [Security].[Privileges] WHERE Operation = {0}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, PrivilegeId));
        }
    }
}
