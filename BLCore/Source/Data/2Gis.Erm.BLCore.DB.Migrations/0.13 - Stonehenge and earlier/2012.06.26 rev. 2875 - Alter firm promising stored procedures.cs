using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(2875, "Изменение загрузки импорта перспективности рекламодателей")]
	public class Migration2875 : TransactedMigration
	{
		private const string ImportFirmPromisingTemplate = @"
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
				PromisingScore = FPD.Value
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

		private const string CalculateClientPromisingTemplate = @"
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
				{0}.dbo.AccountExtensionBase
			SET
				Dg_promisingscore = C.PromisingValue
			FROM
				{0}.dbo.AccountExtensionBase AS CRM_C
				INNER JOIN
				(SELECT ReplicationCode, PromisingValue FROM Billing.Clients WHERE Id IN (SELECT Id FROM @ClientUpdatedIds)) AS C
				ON CRM_C.AccountId = C.ReplicationCode


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

		protected override void ApplyOverride(IMigrationContext context)
		{
			AlterImportFirmPromising(context);
			CreateCalculateClientPromising(context);
		}

		private static void AlterImportFirmPromising(IMigrationContext context)
		{
			var oldName = new SchemaQualifiedObjectName(ErmSchemas.Shared, "ImportFirmsPromissing");

			var oldSp = context.Database.StoredProcedures[oldName.Name, oldName.Schema];
			if (oldSp != null)
				oldSp.Drop();

			var newName = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ImportFirmPromising");
			var spTextBody = string.Format(ImportFirmPromisingTemplate, context.CrmDatabaseName);

			var sp = context.Database.StoredProcedures[newName.Name, newName.Schema];
			if (sp == null)
			{
				sp = new StoredProcedure(context.Database, newName.Name, newName.Schema)
				{
					TextMode = false,
					AnsiNullsStatus = false,
					QuotedIdentifierStatus = false,
					TextBody = spTextBody
				};
				var param = new StoredProcedureParameter(sp, "@OrganizationUnitStableId", DataType.Int) { DefaultValue = "NULL" };
				sp.Parameters.Add(param);

				sp.Create();
			}
            else
			{
                sp.TextBody = spTextBody;
                sp.Alter();
			}
		}

		private static void CreateCalculateClientPromising(IMigrationContext context)
		{
			var name = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CalculateClientPromising");
			var spTextBody = string.Format(CalculateClientPromisingTemplate, context.CrmDatabaseName);

			var sp = context.Database.StoredProcedures[name.Name, name.Schema];
			if (sp == null)
			{
				sp = new StoredProcedure(context.Database, name.Name, name.Schema)
				{
					TextMode = false,
					AnsiNullsStatus = false,
					QuotedIdentifierStatus = false,
					TextBody = spTextBody
				};
				sp.Create();
			}
            else
			{
			    sp.TextBody = spTextBody;
                sp.Alter();
			}
		}
	}
}