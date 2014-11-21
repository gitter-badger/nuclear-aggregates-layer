using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13938, "[ERM-2174]. Perfomance tuning for view [Security].[vwUserTerritoriesOrganizationUnits]")]
    public class Migration13938 : TransactedMigration
    {
        private const string CommandText = @"
-- Из query удалено ROW_NUMBER() OVER (ORDER BY U.id) AS Id, индекс пока не создаем, DISTINCT пока оставлен, т.к. в коде где-то есть, где-то нет, но на текущий момент можно удалить, т.к. все запросы завершаются либо Any(), либо First и т.п.
ALTER VIEW [Security].[vwUserTerritoriesOrganizationUnits]
WITH SCHEMABINDING
AS
SELECT DISTINCT U.Id AS UserId, OU.Id AS OrganizationUnitId, T.Id AS TerritoryId
FROM Security.Users U
INNER JOIN Security.UserOrganizationUnits UOU ON UOU.UserId = U.Id AND U.IsDeleted = 0 AND U.IsActive = 1
INNER JOIN Billing.OrganizationUnits OU ON OU.Id = UOU.OrganizationUnitId AND OU.IsDeleted = 0 AND OU.IsActive = 1
LEFT JOIN Security.UserTerritories UT ON UT.UserId = U.Id AND UT.IsDeleted = 0
LEFT JOIN BusinessDirectory.Territories T ON T.Id = UT.TerritoryId AND T.IsActive = 1
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
