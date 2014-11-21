-- changes
--   24.10.2013, a.rechkalov: Исправил репликацию идентификаторов пользователей
ALTER PROCEDURE [Billing].[ReplicateOrderProcessingRequest]
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
	FROM [Billing].[OrderProcessingRequests] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM DoubleGis_MSCRM.[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM DoubleGis_MSCRM.[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM DoubleGis_MSCRM.[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM DoubleGis_MSCRM.[dbo].[Dg_orderprocessingrequestBase] WHERE [Dg_orderprocessingrequestId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO DoubleGis_MSCRM.[dbo].[Dg_orderprocessingrequestBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_orderprocessingrequestId]
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
			[OPR].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [OPR].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OPR].[ReplicationCode],			-- <Dg_orderprocessingrequestId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[OPR].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [OPR].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OPR].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[OrderProcessingRequests] AS [OPR]
		WHERE [OPR].[Id] = @Id;
		
		INSERT INTO DoubleGis_MSCRM.[dbo].[Dg_orderprocessingrequestExtensionBase]
           ([Dg_orderprocessingrequestId]
           ,[Dg_name]
		   ,[Dg_duedate]
		   ,[Dg_state]
		   ,[dg_renewedorderid]
		   ,[dg_baseorderid]
           ,[dg_sourceorganizationunitid]
		   ,[dg_firmid])
		SELECT
			 [OPR].[ReplicationCode]
			,[OPR].[Title]
			,[OPR].[DueDate]
			,[OPR].[State]
			,(SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OPR].[RenewedOrderId])
			,(SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OPR].[BaseOrderId])
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [OPR].[SourceOrganizationUnitId])
			,(SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [OPR].[FirmId])
		FROM [Billing].[OrderProcessingRequests] AS [OPR]
		WHERE [OPR].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMOPR]
			SET 
			  [DeletionStateCode] = CASE WHEN [OPR].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OPR].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [OPR].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OPR].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM DoubleGis_MSCRM.[dbo].[Dg_orderprocessingrequestBase] AS [CRMOPR]
			INNER JOIN [Billing].[OrderProcessingRequests] AS [OPR] ON [CRMOPR].[Dg_orderprocessingrequestId] = [OPR].[ReplicationCode] AND [OPR].[Id] = @Id;
		
		
		UPDATE [CRMOPR]
			SET 
				[Dg_name] = [OPR].[Title]
				,[Dg_duedate] = [OPR].[DueDate]
				,[Dg_state] = [OPR].[State]
				,[dg_renewedorderid] = (SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OPR].[RenewedOrderId])
				,[dg_baseorderid] = (SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OPR].[BaseOrderId])
				,[dg_sourceorganizationunitid] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [OPR].[SourceOrganizationUnitId])
				,[dg_firmid] = (SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [OPR].[FirmId])
		FROM DoubleGis_MSCRM.[dbo].[Dg_orderprocessingrequestExtensionBase] AS [CRMOPR]
			INNER JOIN [Billing].[OrderProcessingRequests] AS [OPR] ON [CRMOPR].[Dg_orderprocessingrequestId] = [OPR].[ReplicationCode] AND [OPR].[Id] = @Id;		
	END;
	
	RETURN 1;
