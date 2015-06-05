using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201505121131, "Удаление устаревшей привилегии", "y.baranihin")]
    public class Migration201505121131 : TransactedMigration
    {
        private const int AdditionalFirmServiceCode = 220;

        protected override void ApplyOverride(IMigrationContext context)
        {
            const string QueryTemplate = @"delete from [Security].[RolePrivileges] 
                                                where PrivilegeId in (
                                                                        SELECT [Id]
                                                                        FROM [Security].[Privileges] where EntityType = {0}
                                                                      )

                                           delete from [Security].[Privileges] where EntityType = {0}";

            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, AdditionalFirmServiceCode));
        }
    }
}
