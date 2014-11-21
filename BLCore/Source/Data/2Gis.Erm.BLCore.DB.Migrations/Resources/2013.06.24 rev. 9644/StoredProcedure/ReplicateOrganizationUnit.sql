-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateOrganizationUnit]
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



