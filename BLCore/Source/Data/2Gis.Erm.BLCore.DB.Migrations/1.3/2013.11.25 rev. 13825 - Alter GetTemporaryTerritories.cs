using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13825, "alter на GetTemporaryTerritories", "y.baranihin")]
    public sealed class Migration13825 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            #region

            var query = @"
-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   5.06.2013, a.rechkalov: убрал автоматическое создание региональной территории, теперь хранимка кидает исключение
--   24.06.2013, a.rechkalov: замена int -> bigint
--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
--   25.11.2013, y.baranihin: убрал ненужную транзакционность
ALTER PROCEDURE [Shared].[GetTemporaryTerritories](
	@OrganizationUnits Shared.OrganizationUnitsTableType READONLY,
	@ModifiedBy bigint = NULL,
	@RegionalTerritoryLocalName nvarchar(255)
)
AS
BEGIN
		DECLARE @TempTbl TABLE
		(
			TerritoryId bigint, 
			OrganizationUnitId bigint,
			OrganizationUnitName nvarchar(max)
		)

		INSERT INTO @TempTbl SELECT 
			Id, OrganizationUnitId, NULL FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @OrganizationUnits) AND IsActive = 1 AND Name like N'%' + @RegionalTerritoryLocalName + N'%' ORDER BY Id DESC

		INSERT INTO @TempTbl 
		SELECT 
			NULL, 
			CurrentValues.OrganizationUnitId, 
			Billing.OrganizationUnits.Name
		FROM @OrganizationUnits as CurrentValues 
			inner join Billing.OrganizationUnits ON CurrentValues.OrganizationUnitId = Billing.OrganizationUnits.Id 
		WHERE CurrentValues.OrganizationUnitId NOT IN (SELECT DISTINCT OrganizationUnitId FROM @TempTbl);

		WITH CTE AS (
		SELECT OrganizationUnitId, ROW_NUMBER() OVER(PARTITION BY OrganizationUnitId ORDER BY OrganizationUnitId, OrganizationUnitName DESC) rnk
		FROM @TempTbl
		)
	DELETE FROM CTE
	WHERE rnk > 1;
		
		IF EXISTS(SELECT * FROM @TempTbl WHERE TerritoryId IS NULL)
		BEGIN
			declare @OrganizationUnitName nvarchar(255)
			SELECT top 1 @OrganizationUnitName = OrganizationUnitId FROM @TempTbl WHERE TerritoryId IS NULL
			declare @MessageOrganizationUnitInvalidDgppId nvarchar(1024) = N'Региональная территория для подразделения ' + @OrganizationUnitName + N' не создана. Создайте её вручную. Региональная территория должна содержать в названии ' + @RegionalTerritoryLocalName
			RAISERROR(@MessageOrganizationUnitInvalidDgppId, 16, 2)
		END

		SELECT TerritoryId, OrganizationUnitId FROM @TempTbl
END";

            #endregion

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
