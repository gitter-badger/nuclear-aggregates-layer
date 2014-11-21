-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Integration].[ImportFirmPromising]
	@OrganizationUnitStableId [bigint] = NULL,
	@ModifiedBy [bigint] = NULL,
	@EnableReplication [bit] = 1
WITH EXECUTE AS CALLER
AS

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @LatestSessionId bigint
DECLARE @FirmUpdatedIds TABLE(Id bigint PRIMARY KEY)

-- FirmPromissingSessions.Status possible values
-- 0 - New
-- 1 - Processed
-- 2 - Error
-- 3 - Old

BEGIN TRY

	-- самая свежая выгрузка для города, зафейлившиеся выгрузки пытаются загрузиться ещё раз
	SET	@LatestSessionId =
	(
	SELECT TOP 1 Id FROM Shared.FirmPromissingSessions
	WHERE 
		OrganizationUnitId = @OrganizationUnitStableId AND ([Status] = 0 OR [Status] = 2)
	ORDER BY [Date] DESC
	)

	IF(@LatestSessionId IS NULL)
	BEGIN
		RETURN 1;
	END

	-- выгрузки от OLAP не инкрементальны, поэтому более старые выгрузки загружать нет смысла
	UPDATE Shared.FirmPromissingSessions
	SET
		[Status] = 3
	WHERE
		OrganizationUnitId = @OrganizationUnitStableId AND ([Status] = 0 OR [Status] = 2) AND Id <> @LatestSessionId

	DELETE FROM 
		Shared.FirmPromissingDetails
	WHERE
		SessionId IN (SELECT Id FROM Shared.FirmPromissingSessions WHERE OrganizationUnitId = @OrganizationUnitStableId AND Id <> @LatestSessionId)

	BEGIN TRANSACTION		

		UPDATE BusinessDirectory.Firms
		SET
			PromisingScore = FPD.Value,
			ModifiedBy = @ModifiedBy,
			ModifiedOn = GETUTCDATE()
		OUTPUT
			inserted.Id
		INTO
			@FirmUpdatedIds
		FROM
			BusinessDirectory.Firms AS F
			INNER JOIN
			(SELECT FirmId, Value FROM Shared.FirmPromissingDetails WHERE SessionId = @LatestSessionId) AS FPD
			ON FPD.FirmId = F.DgppId

		DELETE FROM
			Shared.FirmPromissingDetails
		WHERE
			SessionId = @LatestSessionId;

		-- репликация фирм в Dynamics CRM
		IF(@EnableReplication = 1)
		BEGIN
			DECLARE @CurrentId bigint
			SELECT @CurrentId = MIN(Id) FROM @FirmUpdatedIds
			WHILE @CurrentId IS NOT NULL
			BEGIN
				EXEC BusinessDirectory.ReplicateFirm @CurrentId

				SELECT @CurrentId = MIN(Id) FROM @FirmUpdatedIds WHERE Id > @CurrentId
			END
		END

		UPDATE Shared.FirmPromissingSessions
		SET
			[Status] =	1 --- Processed
		WHERE 
			Id = @LatestSessionId

	COMMIT TRANSACTION;
		
	RETURN 1;
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
	BEGIN
		ROLLBACK TRANSACTION;
	END;

	UPDATE 
		Shared.FirmPromissingSessions
	SET 
		[Status] = 2 --- Failed
	WHERE
		Id = @LatestSessionId
		
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;


