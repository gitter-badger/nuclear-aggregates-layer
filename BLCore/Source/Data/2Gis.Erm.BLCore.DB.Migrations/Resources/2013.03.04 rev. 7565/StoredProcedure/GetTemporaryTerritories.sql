ALTER PROCEDURE [Shared].[GetTemporaryTerritories](@OrganizationUnits Shared.OrganizationUnitsTableType READONLY, @ModifiedBy Int = NULL, @OwnerCode Int = NULL)
AS
BEGIN
	BEGIN TRY

		SET XACT_ABORT ON;

		BEGIN TRAN

		DECLARE @TempTbl TABLE
		(
			TerritoryId int, 
			OrganizationUnitId int,
			OrganizationUnitName nvarchar(max)
		)

		INSERT INTO @TempTbl SELECT 
			Id, OrganizationUnitId, NULL FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @OrganizationUnits) AND IsActive = 1 AND Name like '%Региональная территория%' ORDER BY DgppId DESC

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

			DECLARE @TerritoryIds TABLE (Id INT NOT NULL, OrganizationUnitId INT NOT NULL)

			INSERT BusinessDirectory.Territories (DgppId, Name, OrganizationUnitId, ReplicationCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
			OUTPUT inserted.Id, inserted.OrganizationUnitId INTO @TerritoryIds
			SELECT NULL, OrganizationUnitName + '. Региональная территория',OrganizationUnitId, NEWID(), @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE()
			FROM @TempTbl WHERE TerritoryId IS NULL
		
			UPDATE @TempTbl 
		SET TerritoryId = NewValues.Id
		FROM @TempTbl CurrentValues inner join @TerritoryIds NewValues ON CurrentValues.OrganizationUnitId = NewValues.OrganizationUnitId
			
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

GO