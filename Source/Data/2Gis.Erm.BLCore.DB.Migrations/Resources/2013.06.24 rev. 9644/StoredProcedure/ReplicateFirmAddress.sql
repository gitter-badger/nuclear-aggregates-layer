-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [BusinessDirectory].[ReplicateFirmAddress]
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



