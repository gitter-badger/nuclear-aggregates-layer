using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7833, "Пересоздание views")]
    public sealed class Migration7833 : TransactedMigration
    {
        private const string SqlStatement = @"
DROP VIEW [Billing].[vwOrganizationUnits]
DROP VIEW [BusinessDirectory].[vwTerritories]
DROP VIEW [Security].[vwUsersDescendants]
DROP VIEW [Security].[vwUserTerritoriesOrganizationUnits]
go

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
go

CREATE view [Billing].[vwOrganizationUnits]
AS
SELECT Id, Name, Code, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn 
FROM Billing.OrganizationUnits 
WHERE IsActive=1 and IsDeleted=0
go

CREATE VIEW [BusinessDirectory].[vwTerritories]
AS
SELECT Id, Name, OrganizationUnitId, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
FROM BusinessDirectory.Territories
WHERE (IsActive = 1)
go

CREATE VIEW [Security].[vwUsersDescendants]
AS
	WITH CTE (AncestorId, DescendantId, [Level]) AS
	(
		SELECT ParentId, Id, 1 AS [Level] FROM [Security].[Users] WHERE ParentId IS NOT NULL AND IsActive = 1 AND IsDeleted = 0
		UNION ALL
		SELECT CTE.AncestorId, U.Id, [Level] + 1 AS [Level] FROM CTE INNER JOIN [Security].[Users] AS U ON U.ParentId = CTE.DescendantId AND U.IsActive = 1 AND U.IsDeleted = 0
	)
	SELECT 
		ISNULL( ROW_NUMBER() OVER (ORDER BY AncestorId, DescendantId), 0 ) AS Id, AncestorId, DescendantId, [Level]
	FROM CTE
go

CREATE VIEW [Security].[vwUserTerritoriesOrganizationUnits]
AS
SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY U.id) AS Id, U.Id AS UserId, OU.Id AS OrganizationUnitId, T.Id AS TerritoryId
                            FROM Security.Users U
                            INNER JOIN Security.UserOrganizationUnits UOU ON UOU.UserId = U.Id
                            INNER JOIN Billing.OrganizationUnits OU ON OU.Id = UOU.OrganizationUnitId AND OU.IsDeleted = 0 AND OU.IsActive = 1
                            LEFT JOIN Security.UserTerritories UT ON UT.UserId = U.Id AND UT.IsDeleted = 0
                            LEFT JOIN BusinessDirectory.Territories T ON T.Id = UT.TerritoryId AND T.IsActive = 1
                            WHERE
                            (U.IsDeleted = 0 AND U.IsActive = 1)
GO
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(SqlStatement);
        }
    }
}
