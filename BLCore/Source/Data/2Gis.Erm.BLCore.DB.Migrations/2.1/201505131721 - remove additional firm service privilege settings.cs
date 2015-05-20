using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201505131721, "Удаление настроек привилегий для дополнительных услаг по фирме", "d.ivanov")]
    public class Migration201505131721 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int AdditionalFirmService = 220;
            context.Database.ExecuteNonQuery(
                string.Format("DELETE FROM [Security].[RolePrivileges] WHERE [PrivilegeId] in (SELECT [Id] FROM [Security].[Privileges] WHERE [EntityType] = {0});" +
                              "DELETE FROM [Security].[Privileges] WHERE [EntityType] = {0}",
                               AdditionalFirmService));
        }
    }
}