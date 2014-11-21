using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5092, "Перестройка vwUserTerritoriesOrganizationUnits")]
    public sealed class Migration5092 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterUserTerritoriesOrganizationUnitsView(context);
        }

        private static void AlterUserTerritoriesOrganizationUnitsView(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserTerritoriesOrganizationUnits", ErmSchemas.Security];
            view.TextBody = @"SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY U.id) AS Id, U.Id AS UserId, OU.Id AS OrganizationUnitId, T.Id AS TerritoryId
                            FROM Security.Users U
                            INNER JOIN Security.UserOrganizationUnits UOU ON UOU.UserId = U.Id
                            INNER JOIN Billing.OrganizationUnits OU ON OU.Id = UOU.OrganizationUnitId AND OU.IsDeleted = 0 AND OU.IsActive = 1
                            LEFT JOIN Security.UserTerritories UT ON UT.UserId = U.Id AND UT.IsDeleted = 0
                            LEFT JOIN BusinessDirectory.Territories T ON T.Id = UT.TerritoryId AND T.IsDeleted = 0 AND T.IsActive = 1
                            WHERE
                            (U.IsDeleted = 0 AND U.IsActive = 1)";
            view.Alter();
        }
   }
}