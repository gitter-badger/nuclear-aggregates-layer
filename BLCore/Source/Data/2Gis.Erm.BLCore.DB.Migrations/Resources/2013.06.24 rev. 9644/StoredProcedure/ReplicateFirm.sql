-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [BusinessDirectory].[ReplicateFirm]
	@Id bigint = NULL
AS

	SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;
		
	SET XACT_ABORT ON;

	DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserDomainName NVARCHAR(250);
    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
    DECLARE @ModifiedByUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserId UNIQUEIDENTIFIER;
	DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Firms] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_firmBase] WHERE [Dg_firmId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_firmBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_firmId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OverriddenCreatedOn]
           ,[OwningBusinessUnit]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
           ,[OwningUser])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[F].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [F].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[F].[ReplicationCode],		-- <Dg_firmId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[F].[ModifiedOn],			-- <ModifiedOn, datetime,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,	-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [F].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [F].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>					
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL,						-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId				-- <OwningUser, uniqueidentifier,>
		FROM [BusinessDirectory].[Firms] AS [F]
		WHERE [F].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_firmExtensionBase]
           ([Dg_account]
		   ,[Dg_closedforascertainment]
           ,[Dg_firmId]
		   ,[Dg_lastqualifytime]
           ,[Dg_lastdisqualifytime]
           ,[Dg_markettype]
           ,[Dg_name]
		   ,[Dg_organizationunit]
           ,[Dg_promisingscore]
		   ,[Dg_territory]
           ,[Dg_usingothermedia]
           ,[Dg_ProductType]
           ,[Dg_BudgetType]
           ,[Dg_Geolocation]
           ,[Dg_InCityBranchesAmount]
           ,[Dg_OutCityBranchesAmount]
           ,[Dg_StaffAmount]
		)
		SELECT
													-- <dg_account, uniqueidentifier,>
			(SELECT [C].[ReplicationCode] FROM [Billing].[Clients] AS [C] WHERE [C].[Id] = [F].[ClientId])
			,[F].[ClosedForAscertainment]			-- <Dg_closedforascertainment, bit,>
			,[F].[ReplicationCode]					-- <Dg_firmId, uniqueidentifier,>
			,[F].[LastQualifyTime]					-- <Dg_lastqualifytime, datetime,>
			,[F].[LastDisqualifyTime]				-- <Dg_lastdisqualifytime, datetime,>
			,[F].[MarketType]						-- <Dg_markettype, int,>
			,[F].[Name]								-- <Dg_name, NVARCHAR(100),>
													-- <Dg_organizationunit, uniqueidentifier,>
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [F].[OrganizationUnitId])
			,[F].[PromisingScore]					-- <Dg_promisingscore, int,>
													-- <dg_territory, uniqueidentifier,>
			,(SELECT [T].[ReplicationCode] FROM [BusinessDirectory].[Territories] AS [T] WHERE [T].[Id] = [F].[TerritoryId])
			,[F].[UsingOtherMedia]					-- <Dg_usingothermedia, int,>
			,[F].[ProductType]						-- <Dg_ProductType, int,>
			,[F].[BudgetType]						-- <Dg_BudgetType, int,>
			,[F].[Geolocation]						-- <Dg_Geolocation, int,>
			,[F].[InCityBranchesAmount]				-- <Dg_InCityBranchesAmount, int,>
			,[F].[OutCityBranchesAmount]			-- <Dg_OutCityBranchesAmount, int,>
			,[F].[StaffAmount]						-- <Dg_StaffAmount, int,>
			
		FROM [BusinessDirectory].[Firms] AS [F]
		WHERE [F].[Id] = @Id;
		
	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMF]
			SET 
			  [DeletionStateCode] = CASE WHEN [F].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [F].[ModifiedOn]
			  --,[OrganizationId] = <OrganizationId, uniqueidentifier,>
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [F].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [F].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_firmBase] AS [CRMF]
			INNER JOIN [BusinessDirectory].[Firms] AS [F] ON [CRMF].[Dg_firmId] = [F].[ReplicationCode] AND [F].[Id] = @Id;
		
		UPDATE [CRMF]
		SET
			   [Dg_account] = (SELECT [C].[ReplicationCode] FROM [Billing].[Clients] AS [C] WHERE [C].[Id] = [F].[ClientId])
			  ,[Dg_closedforascertainment] = [F].[ClosedForAscertainment]
			  ,[Dg_lastqualifytime] = [F].[LastQualifyTime]
			  ,[Dg_lastdisqualifytime] = [F].[LastDisqualifyTime]
			  ,[Dg_markettype] = [F].[MarketType]
			  ,[Dg_name] = [F].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [F].[OrganizationUnitId])
			  ,[Dg_promisingscore] = [F].[PromisingScore]
			  ,[Dg_territory] = (SELECT [T].[ReplicationCode] FROM [BusinessDirectory].[Territories] AS [T] WHERE [T].[Id] = [F].[TerritoryId])
		      ,[Dg_usingothermedia] = [F].[UsingOtherMedia]
		      ,[Dg_ProductType] = [F].[ProductType]
		      ,[Dg_BudgetType] = [F].[BudgetType]
		      ,[Dg_Geolocation] = [F].[Geolocation]
		      ,[Dg_InCityBranchesAmount] = [F].[InCityBranchesAmount]
		      ,[Dg_OutCityBranchesAmount] = [F].[OutCityBranchesAmount]
		      ,[Dg_StaffAmount] = [F].[StaffAmount]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_firmExtensionBase] AS [CRMF]
			INNER JOIN [BusinessDirectory].[Firms] AS [F] ON [CRMF].[Dg_firmId] = [F].[ReplicationCode]	AND [F].[Id] = @Id;
	END;
	
	RETURN 1;


