-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicatePosition]
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



