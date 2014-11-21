using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7089, "Детализации ошибки репликации и правка репликации адресов")]
    public sealed class Migration7089 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"ALTER PROCEDURE [Integration].[ReplicateObjectsAfterImportCards]
AS
BEGIN

SET XACT_ABORT ON;

BEGIN TRY;

-- Удалаяем дубли фирм, нуждающихся в обработке
WITH CTE AS (
	SELECT FirmId, ROW_NUMBER() OVER(PARTITION BY FirmId ORDER BY FirmId, CreatedOn DESC) rnk
	FROM Integration.FirmsForPostIntegrationActivities
	 )
DELETE FROM CTE
WHERE rnk > 1;

--Репликация объектов
BEGIN TRAN
DECLARE @FirmIds Shared.Int32IdsTableType
DECLARE @AddressIds Shared.Int32IdsTableType

INSERT INTO @FirmIds SELECT TOP 1000 FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ReplicateObjects = 0
INSERT INTO @AddressIds SELECT Id FROM BusinessDirectory.FirmAddresses WHERE FirmId IN (SELECT Id FROM @FirmIds)

EXEC BusinessDirectory.ReplicateFirms @FirmIds
EXEC BusinessDirectory.ReplicateFirmAddresses @AddressIds

UPDATE Integration.FirmsForPostIntegrationActivities SET ReplicateObjects = 1 WHERE FirmId IN (SELECT Id FROM @FirmIds)
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1
COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN

		DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
		SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END
GO


ALTER PROCEDURE [BusinessDirectory].[ReplicateFirmAddresses]
	@Ids [Shared].[Int32IdsTableType] readonly
AS

	
	SET NOCOUNT ON;
	
	IF Not Exists (SELECT * FROM @Ids)
		RETURN 0;
		
	SET XACT_ABORT ON;

	DECLARE @ReferenceInfo Table (
	 CrmId UNIQUEIDENTIFIER NULL,
	 CreatedByUserId UNIQUEIDENTIFIER NULL, 
	 CreatedByUserDomainName NVARCHAR(250) NULL, 
	 ModifiedByUserId UNIQUEIDENTIFIER NULL,
	 ModifiedByUserDomainName NVARCHAR(250) NULL,
	 OwnerUserDomainName NVARCHAR(250) NULL,
	 OwnerUserId UNIQUEIDENTIFIER NULL,
	 OwnerUserOrganizationId UNIQUEIDENTIFIER NULL,
	 FirmAddressId int NOT NULL,
	 AlreadyCreated bit)

	INSERT INTO @ReferenceInfo (CrmId, OwnerUserDomainName, CreatedByUserDomainName, ModifiedByUserDomainName, FirmAddressId, AlreadyCreated)

	-- get owner user domain name, CRM replication code
	SELECT [TBL].[ReplicationCode],
           [O].[Account], 
           [C].[Account], 
           [M].[Account],
		   [TBL].[Id],
		   0
	FROM [BusinessDirectory].[FirmAddresses] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] in (SELECT Id FROM @Ids);

	-- get owner user CRM UserId

	UPDATE @ReferenceInfo
    SET OwnerUserId = SystemUserId,
		OwnerUserOrganizationId = OrganizationId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + OwnerUserDomainName
	COLLATE Cyrillic_General_CI_AI

	UPDATE @ReferenceInfo
    SET CreatedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE CreatedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + CreatedByUserDomainName
	COLLATE Cyrillic_General_CI_AI

	UPDATE @ReferenceInfo
    SET ModifiedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE ModifiedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + ModifiedByUserDomainName
	COLLATE Cyrillic_General_CI_AI

	    UPDATE @ReferenceInfo
    SET AlreadyCreated = 1
    FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase]
    WHERE [Dg_firmaddressId] = CrmId;

	INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase]
		   ([CreatedBy]
		   ,[CreatedOn]
		   ,[DeletionStateCode]
		   ,[Dg_firmaddressId]
		   ,[ImportSequenceNumber]
		   ,[ModifiedBy]
		   ,[ModifiedOn]
		   ,[OrganizationId]
		   ,[OverriddenCreatedOn]
		   ,[statecode]
		   ,[statuscode]
		   ,[TimeZoneRuleVersionNumber]
		   ,[UTCConversionTimeZoneCode])
	SELECT
			info.CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[FA].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[FA].[ReplicationCode],				-- <Dg_firmaddressId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(info.ModifiedByUserId, info.CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[FA].[ModifiedOn],			-- <ModifiedOn, datetime,>
			info.OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
	FROM [BusinessDirectory].[FirmAddresses] AS [FA]
		inner join @ReferenceInfo info ON FA.Id = info.FirmAddressId 
	WHERE info.AlreadyCreated = 0
		
	INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_firmaddressExtensionBase]
		   ([Dg_address]
		   ,[Dg_firmaddressId]
		   ,[Dg_closedforascertainment]
		   ,[Dg_firm])
	SELECT
			 [FA].[Address]
			,[FA].[ReplicationCode]
			,[FA].[ClosedForAscertainment]
			,(SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [FA].[FirmId])
	FROM [BusinessDirectory].[FirmAddresses] AS [FA]
	inner join @ReferenceInfo info ON FA.Id = info.FirmAddressId 
	WHERE info.AlreadyCreated = 0;
		
	INSERT INTO [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_firmaddressBase]
		   ([dg_dg_category_dg_firmaddressId]
		   ,[dg_categoryid]
		   ,[dg_firmaddressid])
	SELECT
			NEWID(),
			[C].[ReplicationCode],
			[FA].[ReplicationCode]
	FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
	WHERE [CFA].[FirmAddressId] in (SELECT FirmAddressId FROM @ReferenceInfo WHERE AlreadyCreated = 0);

	

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
		
	UPDATE [CRMFA]
			SET 
			  [DeletionStateCode] = CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL([INFO].ModifiedByUserId, [INFO].CreatedByUserId)
			  ,[ModifiedOn] = [FA].[ModifiedOn]
			  --,[OrganizationId] = <OrganizationId, uniqueidentifier,>
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
	FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode]
			INNER JOIN @ReferenceInfo INFO ON FA.Id = INFO.FirmAddressId AND [INFO].[AlreadyCreated] = 1
		
		
	UPDATE [CRMFA]
		   SET
			   [Dg_address] = [FA].[Address]
			  ,[Dg_closedforascertainment] = [FA].[ClosedForAscertainment]
			  ,[Dg_firm]    = (SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [FA].[FirmId])
	FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressExtensionBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode] AND [FA].[Id] in (SELECT FirmAddressId FROM @ReferenceInfo WHERE AlreadyCreated = 1);
				
		------------------------------------------------------------------------------------------------------------------------
		---------------------------------------- * dg_dg_category_dg_firmaddressBase * -----------------------------------------
		------------------------------------------------------------------------------------------------------------------------
	INSERT INTO [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_firmaddressBase]
		   ([dg_dg_category_dg_firmaddressId]
		   ,[dg_categoryid]
		   ,[dg_firmaddressid])
	SELECT
			NEWID(),
			[C].[ReplicationCode],
			[FA].[ReplicationCode]
	FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
	WHERE [CFA].[FirmAddressId] in (SELECT FirmAddressId FROM @ReferenceInfo WHERE AlreadyCreated = 1)
			AND [CFA].[IsDeleted] = 0
			AND NOT EXISTS (
				SELECT 1
				FROM [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA]
				WHERE [CRMFA].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA].[dg_firmaddressid] = [FA].[ReplicationCode]
			);

	DELETE [CRMFA]
	FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
			INNER JOIN [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA]
				ON	[CRMFA].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA].[dg_firmaddressid] = [FA].[ReplicationCode]
	WHERE [CFA].[FirmAddressId] in (SELECT FirmAddressId FROM @ReferenceInfo WHERE AlreadyCreated = 1)
			AND [CFA].[IsDeleted] = 1
			AND EXISTS (
				SELECT 1
				FROM [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA2]
				WHERE [CRMFA2].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA2].[dg_firmaddressid] = [FA].[ReplicationCode]
			);
		------------------------------------------------------------------------------------------------------------------------
		---------------------------------------- * dg_dg_category_dg_firmaddressBase * -----------------------------------------
		------------------------------------------------------------------------------------------------------------------------
	
	RETURN 1;
GO

";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
