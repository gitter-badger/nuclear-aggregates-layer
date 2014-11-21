using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22874, "Удаляю лишнюю ф. привелегию", "y.baranikhin")]
    public class Migration22874 : TransactedMigration
    {
        public const int AdvertisementElementStatusEntity = 316;
        public const int DeleteOperation = 65536;

        private const string DeleteStatementTemplate = @"
delete from [Security].[RolePrivileges] where PrivilegeId in (select Id from [Security].[Privileges] where EntityType = {0} and Operation = {1}); 
delete from [Security].[Privileges] where EntityType = {0} and Operation = {1}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate,
                                                           AdvertisementElementStatusEntity,
                                                           DeleteOperation));
        }
    }
}