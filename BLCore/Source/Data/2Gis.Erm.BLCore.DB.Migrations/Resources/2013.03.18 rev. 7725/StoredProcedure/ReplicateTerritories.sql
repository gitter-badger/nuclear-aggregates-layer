ALTER PROCEDURE [BusinessDirectory].[ReplicateTerritories]
	@Ids [Shared].[Int32IdsTableType] ReadOnly
AS

	SET NOCOUNT ON;
	
	IF NOT EXISTS(SELECT * FROM @Ids)
		RETURN 0;
		
	SET XACT_ABORT ON;

		BEGIN TRY
	
	BEGIN TRAN

DECLARE @ReferenceInfo Table (
	 CrmId UNIQUEIDENTIFIER NULL,
	 CreatedByUserId UNIQUEIDENTIFIER NULL, 
	 CreatedByUserDomainName NVARCHAR(250) NULL, 
	 ModifiedByUserId UNIQUEIDENTIFIER NULL,
	 ModifiedByUserDomainName NVARCHAR(250) NULL,
	 OwnerUserDomainName NVARCHAR(250) NULL,
	 OwnerUserId UNIQUEIDENTIFIER NULL,
	 OwnerUserOrganizationId UNIQUEIDENTIFIER NULL,
	 TerritoryId int NOT NULL,
	 AlreadyCreated bit)

	-- get owner user domain name, CRM replication code
	INSERT INTO @ReferenceInfo (CrmId, OwnerUserDomainName, CreatedByUserDomainName, ModifiedByUserDomainName, TerritoryId, AlreadyCreated)
	SELECT [TBL].[ReplicationCode],
           [O].[Account], 
           [C].[Account], 
           [M].[Account],
		   [TBL].[Id],
		   0
	FROM [BusinessDirectory].[Territories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] in (SELECT Id FROM @Ids);
    
	-- get owner user CRM UserId
	UPDATE @ReferenceInfo
    SET OwnerUserId = SystemUserId,
		OwnerUserOrganizationId = OrganizationId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + OwnerUserDomainName
	COLLATE Cyrillic_General_CI_AS;

    -- get CreatedBy user CRM UserId
	UPDATE @ReferenceInfo
    SET CreatedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE CreatedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + CreatedByUserDomainName
	COLLATE Cyrillic_General_CI_AS;

    -- get ModifiedBy user CRM UserId
	UPDATE @ReferenceInfo
    SET ModifiedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE ModifiedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + ModifiedByUserDomainName
	COLLATE Cyrillic_General_CI_AS;

	UPDATE @ReferenceInfo
    SET AlreadyCreated = 1
    FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase]
    WHERE [Dg_territoryId] = CrmId;

	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_territoryBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_territoryId]
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
			[INFO].CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[T].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 0 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[T].[ReplicationCode],		-- <Dg_territoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL([INFO].ModifiedByUserId, [INFO].CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[T].[ModifiedOn],			-- <ModifiedOn, datetime,>
			[INFO].OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
	FROM [BusinessDirectory].[Territories] AS [T]
		inner join @ReferenceInfo INFO ON T.Id = INFO.TerritoryId
	WHERE INFO.AlreadyCreated = 0;
				
	INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase]
           ([Dg_name]
           ,[Dg_territoryId]
           ,[dg_organizationunit])
	SELECT
			 [T].[Name]
			,[T].[ReplicationCode]
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
	FROM [BusinessDirectory].[Territories] AS [T]
		inner join @ReferenceInfo INFO ON T.Id = INFO.TerritoryId
	WHERE INFO.AlreadyCreated = 0;

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------	
		
	UPDATE [CRMT]
	SET 
			  [DeletionStateCode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 2 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(INFO.ModifiedByUserId, INFO.CreatedByUserId)
			  ,[ModifiedOn] = [T].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
	FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] 
			INNER JOIN @ReferenceInfo INFO ON T.Id = INFO.TerritoryId AND [INFO].[AlreadyCreated] = 1;		
		
	UPDATE [CRMT]
	SET 
		       [Dg_name] = [T].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
	FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] in (SELECT TerritoryId FROM @ReferenceInfo WHERE AlreadyCreated = 1);
	
	COMMIT TRAN

	RETURN 1;
END TRY
BEGIN CATCH
       IF (XACT_STATE() != 0)
             ROLLBACK TRAN

       DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
       SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
       RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
GO
