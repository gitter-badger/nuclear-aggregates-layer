-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [BusinessDirectory].[ReplicateCategory]
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



