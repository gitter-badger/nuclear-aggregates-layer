using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201411240501, "Удаление ФП Формирования исходящей региональной рекламы", "y.baranihin")]
    public class Migration201411240501 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int MakeRegionalAdsDocsFp = 536;

            const string QueryTemplate = @"Delete from [Security].[RolePrivileges] where PrivilegeId in 
                                              (Select Id From [Security].[Privileges] Where Operation = {0})

                                           Delete from [Security].[FunctionalPrivilegeDepths] where PrivilegeId in 
                                              (Select Id From [Security].[Privileges] Where Operation = {0})
        
                                           Delete From [Security].[Privileges] Where Operation = {0}";

            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, MakeRegionalAdsDocsFp));
        }
    }
}