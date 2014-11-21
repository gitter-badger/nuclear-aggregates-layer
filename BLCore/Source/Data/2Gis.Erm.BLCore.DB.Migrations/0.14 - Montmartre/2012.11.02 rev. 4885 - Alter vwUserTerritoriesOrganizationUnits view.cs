using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4885, "Alter на Security.vwUserTerritoriesOrganizationUnits")]
    public sealed class Migration4885 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterView1(context);
        }

        private static void AlterView1(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserTerritoriesOrganizationUnits", ErmSchemas.Security];
            view.TextBody = @"SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY u.id) AS Id, u.Id AS UserId, uou.OrganizationUnitId, ut.TerritoryId
                         FROM [Security].Users u INNER JOIN
                         [Security].UserOrganizationUnits uou ON u.Id = uou.UserId AND u.IsDeleted = 0 AND u.IsActive = 1 INNER JOIN
                         [Billing].OrganizationUnits ou ON uou.OrganizationUnitId = ou.Id AND ou.IsDeleted = 0 AND ou.IsActive = 1 LEFT JOIN
                         [Security].UserTerritories ut ON u.Id = ut.UserId LEFT JOIN
                         [BusinessDirectory].Territories t ON t .Id = ut.TerritoryId AND t .IsDeleted = 0 AND t .IsActive = 1";
            view.Alter();
        }
    }
}