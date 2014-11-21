using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2611, "апдейт хранимой процедуры ImportFirmsPromissing")]
    public class Migration2611 : TransactedMigration
    {
        #region SQL statement

        private const string ImportFirmsPromissingSpTemplate = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @SessionId INT
	DECLARE @FirmUpdatedIds TABLE(Id INT PRIMARY KEY)
	DECLARE @ClientUpdatedIds TABLE(Id INT PRIMARY KEY)

	BEGIN TRY

		SELECT @SessionId = MAX([FPS].[Id])
		FROM
			[Shared].[FirmPromissingSessions] AS [FPS]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] 
			ON [FPS].[OrganizationUnitId] = [OU].[DgppId] AND [FPS].[Status] = 0 --- New
		WHERE
			[OU].[DgppId] = @OrganizationUnitId
		
		IF(@SessionId IS NULL)
		BEGIN
			RETURN 1;
		END
		
		BEGIN TRANSACTION		

			UPDATE BusinessDirectory.Firms
			SET
				PromisingScore = FPD.Value
			OUTPUT
				inserted.Id
			INTO
				@FirmUpdatedIds
			FROM
				BusinessDirectory.Firms AS F
				INNER JOIN
				(SELECT FirmId, Value FROM Shared.FirmPromissingDetails WHERE SessionId = @SessionId) AS FPD
				ON FPD.FirmId = F.DgppId

			DELETE FROM
				Shared.FirmPromissingDetails
			WHERE
				SessionId = @SessionId;

			WITH ClientStats(Id, MaxPromisingScore) AS
			(
				SELECT C.Id, MAX(F.PromisingScore) FROM Billing.Clients AS C
				INNER JOIN
				(SELECT * FROM BusinessDirectory.Firms WHERE Id IN (SELECT Id From @FirmUpdatedIds)) AS F
				ON F.ClientId = C.Id
				GROUP BY C.Id
			)
			UPDATE Billing.Clients
			SET
				PromisingValue = ClientStats.MaxPromisingScore
			OUTPUT
				inserted.Id
			INTO
				@ClientUpdatedIds
			FROM
				Billing.Clients
				INNER JOIN
				ClientStats
				ON Clients.Id = ClientStats.Id

			-- replicate to dynamics crm
			UPDATE
				{0}.dbo.Dg_firmExtensionBase
			SET
				Dg_promisingscore = F.PromisingScore
			FROM
				{0}.dbo.Dg_firmExtensionBase AS CRM_F
				INNER JOIN
				(SELECT ReplicationCode, PromisingScore FROM BusinessDirectory.Firms WHERE Id IN (SELECT Id FROM @FirmUpdatedIds)) AS F
				ON CRM_F.Dg_firmId = F.ReplicationCode;

			UPDATE
				{0}.dbo.AccountExtensionBase
			SET
				Dg_promisingscore = C.PromisingValue
			FROM
				{0}.dbo.AccountExtensionBase AS CRM_C
				INNER JOIN
				(SELECT ReplicationCode, PromisingValue FROM Billing.Clients WHERE Id IN (SELECT Id FROM @ClientUpdatedIds)) AS C
				ON CRM_C.AccountId = C.ReplicationCode

			UPDATE Shared.FirmPromissingSessions
			SET
				[Status] =	1 --- Processed
			WHERE 
				Id = @SessionId

		COMMIT TRANSACTION;
		
		RETURN 1;
	END TRY
	BEGIN CATCH
		IF (XACT_STATE() != 0)
		BEGIN
	        ROLLBACK TRANSACTION;
		END;

		UPDATE 
			[Shared].[FirmPromissingSessions] 
		SET 
			[Status] = 2 --- Failed
		WHERE
			Id = @SessionId
		
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH;";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var importFirmsPromissing = new SchemaQualifiedObjectName(ErmSchemas.Shared, "ImportFirmsPromissing");

            var spTextBody = string.Format(ImportFirmsPromissingSpTemplate, context.CrmDatabaseName);

            ReplicationHelper.UpdateOrCreateReplicationSP(context, importFirmsPromissing, spTextBody);
        }
    }
}
