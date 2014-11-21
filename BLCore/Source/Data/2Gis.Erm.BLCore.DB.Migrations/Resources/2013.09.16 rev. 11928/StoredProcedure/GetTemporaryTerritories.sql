-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   5.06.2013, a.rechkalov: убрал автоматическое создание региональной территории, теперь хранимка кидает исключение
--   24.06.2013, a.rechkalov: замена int -> bigint
--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Shared].[GetTemporaryTerritories](
	@OrganizationUnits Shared.OrganizationUnitsTableType READONLY,
	@ModifiedBy bigint = NULL,
	@RegionalTerritoryLocalName nvarchar(255)
)
AS
BEGIN
	BEGIN TRY

		SET XACT_ABORT ON;

		BEGIN TRAN

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

			--DECLARE @TerritoryIds TABLE (Id INT NOT NULL, OrganizationUnitId INT NOT NULL)

			--INSERT BusinessDirectory.Territories (DgppId, Name, OrganizationUnitId, ReplicationCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
			--OUTPUT inserted.Id, inserted.OrganizationUnitId INTO @TerritoryIds
			--SELECT NULL, OrganizationUnitName + '. ' + @RegionalTerritoryLocalName, OrganizationUnitId, NEWID(), @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE()
			--FROM @TempTbl WHERE TerritoryId IS NULL
		
			--UPDATE @TempTbl 
			--SET TerritoryId = NewValues.Id
			--FROM @TempTbl CurrentValues inner join @TerritoryIds NewValues ON CurrentValues.OrganizationUnitId = NewValues.OrganizationUnitId
		END

		COMMIT TRAN

		SELECT TerritoryId, OrganizationUnitId FROM @TempTbl
	END TRY
	BEGIN CATCH
		IF (XACT_STATE() != 0)
			ROLLBACK TRAN

		DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
		SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	END CATCH
END
