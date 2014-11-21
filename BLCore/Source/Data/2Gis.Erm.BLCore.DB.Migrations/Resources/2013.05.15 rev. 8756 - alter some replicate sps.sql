ALTER PROCEDURE [BusinessDirectory].[ReplicateFirmAddress]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
		   @OwnerUserDomainName = [O].[Account], 
		   @CreatedByUserDomainName = [C].[Account], 
		   @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[FirmAddresses] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy] 
		LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
		LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
	WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase] WHERE [Dg_firmaddressId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

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
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[FA].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[FA].[ReplicationCode],				-- <Dg_firmaddressId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[FA].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[FirmAddresses] AS [FA]
		WHERE [FA].[Id] = @Id;
		
		
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
		WHERE [FA].[Id] = @Id;
		
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
		WHERE [CFA].[FirmAddressId] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMFA]
			SET 
			  [DeletionStateCode] = CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [FA].[ModifiedOn]
			  --,[OrganizationId] = <OrganizationId, uniqueidentifier,>
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode] AND [FA].[Id] = @Id;
		
		
		UPDATE [CRMFA]
		   SET
			   [Dg_address] = [FA].[Address]
			  ,[Dg_closedforascertainment] = [FA].[ClosedForAscertainment]
			  ,[Dg_firm]    = (SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [FA].[FirmId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_firmaddressExtensionBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode] AND [FA].[Id] = @Id;
				
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
		WHERE [CFA].[FirmAddressId] = @Id
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
		WHERE [CFA].[FirmAddressId] = @Id
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
		
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [BusinessDirectory].[ReplicateCategory]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Categories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_categoryBase] WHERE [Dg_categoryId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_categoryBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_categoryId]
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
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[C].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [C].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[C].[ReplicationCode],		-- <Dg_categoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[C].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [C].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [C].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[Categories] AS [C]
		WHERE [C].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_categoryExtensionBase]
           ([Dg_categoryId]
           ,[Dg_level]
           ,[Dg_name]
           ,[dg_parentcategory])
		SELECT
			 [C].[ReplicationCode]
			,[C].[Level]				-- <Dg_level, int,>
			,[C].[Name]					-- <Dg_name, int,>
										-- <dg_parentcategory, uniqueidentifier,>
			,(SELECT [PC].[ReplicationCode] FROM [BusinessDirectory].[Categories] AS [PC] WHERE [PC].[Id] = [C].[ParentId])
		FROM [BusinessDirectory].[Categories] AS [C]
		WHERE [C].[Id] = @Id;


		INSERT INTO [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase]
           ([dg_dg_category_dg_organizationunitId]
           ,[dg_categoryid]
           ,[dg_organizationunitid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[OU].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [COU].[CategoryId]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [OU].[Id] = [COU].[OrganizationUnitId]
		WHERE [COU].[CategoryId] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMC]
			SET 
			  [DeletionStateCode] = CASE WHEN [C].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [C].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [C].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [C].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [DoubleGis_MSCRM].[dbo].[Dg_categoryBase] AS [CRMC]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMC].[Dg_categoryId] = [C].[ReplicationCode] AND [C].[Id] = @Id;
		
		
		UPDATE [CRMC]
		   SET
			   [Dg_level] = [C].[Level]
			  ,[Dg_name]  = [C].[Name]
			  ,[dg_parentcategory] = (SELECT [PC].[ReplicationCode] FROM [BusinessDirectory].[Categories] AS [PC] WHERE [PC].[Id] = [C].[ParentId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_categoryExtensionBase] AS [CRMC]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMC].[Dg_categoryId] = [C].[ReplicationCode] AND [C].[Id] = @Id;
		
		-----------------------------------------------------------------------------------------------------------------------------
		------------------------- [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase] ------------------------
		-----------------------------------------------------------------------------------------------------------------------------
		INSERT INTO [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase]
           ([dg_dg_category_dg_organizationunitId]
           ,[dg_categoryid]
           ,[dg_organizationunitid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[OU].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [COU].[CategoryId]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [OU].[Id] = [COU].[OrganizationUnitId]
		WHERE [COU].[CategoryId] = @Id
			AND [COU].IsDeleted = 0
			AND NOT EXISTS (
				SELECT 1 
				FROM [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase] AS [CRMCOU]
				WHERE [CRMCOU].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMCOU].[dg_organizationunitid] = [OU].[ReplicationCode]
			)

		DELETE [CRMCOU]
		FROM [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase] AS [CRMCOU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMCOU].[dg_categoryid] = [C].[ReplicationCode]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMCOU].[dg_organizationunitid] = [OU].[ReplicationCode]
			INNER JOIN [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
				ON [COU].[CategoryId] = [C].[Id] AND [COU].[OrganizationUnitId] = [OU].[Id]
		WHERE [COU].[IsDeleted] = 1;
		-----------------------------------------------------------------------------------------------------------------------------
		------------------------- [DoubleGis_MSCRM].[dbo].[dg_dg_category_dg_organizationunitBase] ------------------------
		-----------------------------------------------------------------------------------------------------------------------------
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [Billing].[ReplicateCurrency]
	@Id INT = NULL
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
	FROM [Billing].[Currencies] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_currencyBase] WHERE [Dg_currencyId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_currencyBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_currencyId]
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
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[CR].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [CR].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[CR].[ReplicationCode],			-- <Dg_currencyId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[CR].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [CR].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [CR].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[Currencies] AS [CR]
		WHERE [CR].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_currencyExtensionBase]
           ([Dg_name]
		   ,[Dg_currencysymbol]
		   ,[Dg_isbase]
           ,[Dg_currencyId])
		SELECT
			 [CR].[Name]				-- <Dg_name, NVARCHAR(100),>
			,[CR].[Symbol]				-- <Dg_currencysymbol, NVARCHAR(100),>
			,[CR].[IsBase]				-- <Dg_isbase, NVARCHAR(100),>
			,[CR].[ReplicationCode]		-- <Dg_currencyId, uniqueidentifier,>
		FROM [Billing].[Currencies] AS [CR]
		WHERE [CR].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMCR]
			SET 
			  [DeletionStateCode] = CASE WHEN [CR].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [CR].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [CR].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [CR].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
              ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_currencyBase] AS [CRMCR]
			INNER JOIN [Billing].[Currencies] AS [CR] ON [CRMCR].[Dg_currencyId] = [CR].[ReplicationCode] AND [CR].[Id] = @Id;
		
		UPDATE [CRMCR]
		SET 
		      [Dg_name] = [CR].[Name]
			 ,[Dg_currencysymbol] = [CR].[Symbol]
		     ,[Dg_isbase] = [CR].[IsBase]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_currencyExtensionBase] AS [CRMCR]
			INNER JOIN [Billing].[Currencies] AS [CR] ON [CRMCR].[Dg_currencyId] = [CR].[ReplicationCode] AND [CR].[Id] = @Id;
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [Billing].[ReplicatePosition]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;
	DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[Positions] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId],
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_positionBase] WHERE [Dg_positionId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_positionBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_positionId]
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
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[P].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [P].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[P].[ReplicationCode],			-- <Dg_positionId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[P].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,		-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [P].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [P].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[Positions] AS [P]
		WHERE [P].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_positionExtensionBase]
           ([Dg_name]
           ,[Dg_positionId])
		SELECT
			 [P].[Name]					-- <Dg_name, NVARCHAR(100),>
			,[P].[ReplicationCode]		-- <Dg_positionId, uniqueidentifier,>
		FROM [Billing].[Positions] AS [P]
		WHERE [P].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMP]
			SET 
			  [DeletionStateCode] = CASE WHEN [P].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [P].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [P].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [P].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
              ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_positionBase] AS [CRMP]
			INNER JOIN [Billing].[Positions] AS [P] ON [CRMP].[Dg_positionId] = [P].[ReplicationCode] AND [P].[Id] = @Id;
		
		UPDATE [CRMP]
		SET 
		      [Dg_name] = [P].[Name]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_positionExtensionBase] AS [CRMP]
			INNER JOIN [Billing].[Positions] AS [P] ON [CRMP].[Dg_positionId] = [P].[ReplicationCode] AND [P].[Id] = @Id;
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [Billing].[ReplicateOrganizationUnit]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[OrganizationUnits] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_organizationunitBase] WHERE [Dg_organizationunitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_organizationunitBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_organizationunitId]
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
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[OU].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OU].[ReplicationCode],		-- <Dg_organizationunitId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[OU].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_organizationunitExtensionBase]
           ([Dg_name]
           ,[Dg_organizationunitId]
		   ,[Dg_dgppid]
		   ,[Dg_ElectronicMedia])
		SELECT
			 [OU].[Name]				-- <Dg_name, NVARCHAR(100),>
			,[OU].[ReplicationCode]		-- <Dg_organizationunitId, uniqueidentifier,>
			,[OU].[DgppId]				-- <Dg_dgppid, int,>
			,[OU].[ElectronicMedia]		-- <Dg_ElectronicMedia, NVARCHAR(50),>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMOU]
			SET 
			  [DeletionStateCode] = CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OU].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [DoubleGis_MSCRM].[dbo].[Dg_organizationunitBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
		
		UPDATE [CRMOU]
		SET 
		      [Dg_name] = [OU].[Name]
			  ,[Dg_dgppid] = [OU].[DgppId]
			  ,[Dg_ElectronicMedia] = [OU].[ElectronicMedia]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_organizationunitExtensionBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [Billing].[ReplicateBranchOffice]
	@Id INT = NULL
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
	FROM [Billing].[BranchOffices] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_branchofficeBase] WHERE [Dg_branchofficeId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_branchofficeBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_branchofficeId]
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
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[BO].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [BO].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[BO].[ReplicationCode],			-- <Dg_branchofficeId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[BO].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [BO].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [BO].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[BranchOffices] AS [BO]
		WHERE [BO].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_branchofficeExtensionBase]
           ([Dg_branchofficeId]
           ,[Dg_INN]
           ,[Dg_legalname]
		   )
		SELECT
			 [BO].[ReplicationCode]
			,[BO].[Inn]		-- <Dg_inn, NVARCHAR(100),>
			,[BO].[Name]		-- <Dg_legalname, NVARCHAR(256),>
		FROM [Billing].[BranchOffices] AS [BO]
		WHERE [BO].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMBO]
			SET 
			  [DeletionStateCode] = CASE WHEN [BO].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [BO].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [BO].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [BO].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_branchofficeBase] AS [CRMBO]
			INNER JOIN [Billing].[BranchOffices] AS [BO] ON [CRMBO].[Dg_branchofficeId] = [BO].[ReplicationCode] AND [BO].[Id] = @Id;
		
		
		UPDATE [CRMBO]
			SET 
			   [Dg_INN] = [BO].[Inn]
			  ,[Dg_legalname] = [BO].[Name]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_branchofficeExtensionBase] AS [CRMBO]
			INNER JOIN [Billing].[BranchOffices] AS [BO] ON [CRMBO].[Dg_branchofficeId] = [BO].[ReplicationCode] AND [BO].[Id] = @Id;		
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [Billing].[ReplicateOperationType]
	@ID [int] = NULL
WITH EXECUTE AS CALLER
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
	FROM [Billing].[OperationTypes] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_operationtypeBase] WHERE [Dg_operationtypeId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_operationtypeBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_operationtypeId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
		   ,[OwningUser]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
		   )
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[OT].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OT].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[OT].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			@OwnerUserId,
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL				-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_operationtypeExtensionBase]
			([Dg_operationtypeId]
           ,[Dg_name]
           ,[Dg_IsPlus])
		SELECT 
			 [OT].[ReplicationCode]
			,[OT].[Name]
			,[OT].[IsPlus]
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OT].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
              ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_operationtypeBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
		

		UPDATE [CRMAC]
			SET 
			 [Dg_name] = [OT].[Name]
			,[Dg_IsPlus] = [OT].[IsPlus]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_operationtypeExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
	END;
	
	RETURN 1;
GO

ALTER PROCEDURE [BusinessDirectory].[ReplicateTerritory]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Territories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;
    
	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] WHERE [Dg_territoryId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

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
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[T].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 0 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[T].[ReplicationCode],		-- <Dg_territoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[T].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase]
           ([Dg_name]
           ,[Dg_territoryId]
           ,[dg_organizationunit])
		SELECT
			 [T].[Name]
			,[T].[ReplicationCode]
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMT]
			SET 
			  [DeletionStateCode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 2 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [T].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
		
		
		UPDATE [CRMT]
		   SET 
		       [Dg_name] = [T].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
	END;
	
	RETURN 1;
GO