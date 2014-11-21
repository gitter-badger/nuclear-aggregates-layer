using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5188, "Alter хранимых процедур ImportFirmPromising и CalculateClientPromising")]
    public sealed class Migration5188 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterImportFirmPromising(context);
            AlterCalculateClientPromising(context);
        }

        private static void AlterCalculateClientPromising(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["CalculateClientPromising", ErmSchemas.Integration];
            sp.TextMode = false;

            var param1 = sp.Parameters["@ModifiedBy"];
            if (param1 == null)
            {
                param1 = new StoredProcedureParameter(sp, "@ModifiedBy", DataType.Int) { DefaultValue = "NULL" };
                sp.Parameters.Add(param1);
            }

            var param3 = sp.Parameters["@EnableReplication"];
            if (param3 == null)
            {
                param3 = new StoredProcedureParameter(sp, "@EnableReplication", DataType.Bit) { DefaultValue = "1" };
                sp.Parameters.Add(param3);
            }

            sp.TextBody = @"
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ClientUpdatedIds TABLE(Id INT PRIMARY KEY)

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
			DECLARE @CurrentId INT
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
END CATCH;";

            sp.Alter();
        }

        private static void AlterImportFirmPromising(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ImportFirmPromising", ErmSchemas.Integration];
            sp.TextMode = false;

            var param1 = sp.Parameters["@ModifiedBy"];
            if (param1 == null)
            {
                param1 = new StoredProcedureParameter(sp, "@ModifiedBy", DataType.Int) {DefaultValue = "NULL"};
                sp.Parameters.Add(param1);
            }

            var param3 = sp.Parameters["@EnableReplication"];
            if (param3 == null)
            {
                param3 = new StoredProcedureParameter(sp, "@EnableReplication", DataType.Bit) { DefaultValue = "1" };
                sp.Parameters.Add(param3);
            }

            sp.TextBody = @"
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @LatestSessionId INT
DECLARE @FirmUpdatedIds TABLE(Id INT PRIMARY KEY)

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
			DECLARE @CurrentId INT
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
END CATCH;";

            sp.Alter();
        }
    }
}