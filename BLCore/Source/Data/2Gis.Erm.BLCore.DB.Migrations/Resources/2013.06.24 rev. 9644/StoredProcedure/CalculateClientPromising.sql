-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Integration].[CalculateClientPromising]
	@ModifiedBy bigint = NULL,
	@EnableReplication [bit] = 1
WITH EXECUTE AS CALLER
AS

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ClientUpdatedIds TABLE(Id bigint PRIMARY KEY)

BEGIN TRY
	BEGIN TRANSACTION;	

		WITH ClientStats(Id, MaxPromisingScore) AS
		(
			SELECT C.Id, MAX(F.PromisingScore) FROM Billing.Clients AS C
			INNER JOIN
			BusinessDirectory.Firms AS F
			ON F.ClientId = C.Id
			GROUP BY C.Id
		)
		UPDATE Billing.Clients
		SET
			PromisingValue = ClientStats.MaxPromisingScore,
			ModifiedBy = @ModifiedBy,
			ModifiedOn = GETUTCDATE()
		OUTPUT
			inserted.Id
		INTO
			@ClientUpdatedIds
		FROM
			Billing.Clients
			INNER JOIN
			ClientStats
			ON Clients.Id = ClientStats.Id

		-- репликация фирм в Dynamics CRM
		IF(@EnableReplication = 1)
		BEGIN
			DECLARE @CurrentId bigint
			SELECT @CurrentId = MIN(Id) FROM @ClientUpdatedIds
			WHILE @CurrentId IS NOT NULL
			BEGIN
				EXEC Billing.ReplicateClient @CurrentId

				SELECT @CurrentId = MIN(Id) FROM @ClientUpdatedIds WHERE Id > @CurrentId
			END
		END

	COMMIT TRANSACTION;
	RETURN 1;
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
	BEGIN
		ROLLBACK TRANSACTION;
	END;

	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;


