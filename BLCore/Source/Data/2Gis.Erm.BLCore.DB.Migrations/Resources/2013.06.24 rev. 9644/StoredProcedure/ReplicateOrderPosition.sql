-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateOrderPosition]
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
	FROM [Billing].[OrderPositions] AS [TBL]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_orderpositionBase] WHERE [Dg_orderpositionId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_orderpositionBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_orderpositionId]
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
			[OP].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [OP].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OP].[ReplicationCode],			-- <Dg_orderpositionId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[OP].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [OP].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OP].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[OrderPositions] AS [OP]
		WHERE [OP].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_orderpositionExtensionBase]
           ([Dg_orderpositionId]
           ,[Dg_amount]
		   ,[Dg_discountinpercent]
		   ,[Dg_CategoryRate]
		   ,[Dg_order]
           ,[Dg_position])
		SELECT
			 [OP].[ReplicationCode]
			,[OP].[Amount]
			,[OP].[DiscountPercent]				
			,[OP].[CategoryRate]	
			,(SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OP].[OrderId])
			,(SELECT [P].[ReplicationCode] FROM [Billing].[Positions] AS [P] 
					INNER JOIN [Billing].[PricePositions] [PP] ON [PP].[PositionId] = [P].[Id] AND [PP].[Id] = [OP].[PricePositionId])
		FROM [Billing].[OrderPositions] AS [OP]
		WHERE [OP].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMOP]
			SET 
			  [DeletionStateCode] = CASE WHEN [OP].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OP].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [OP].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OP].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_orderpositionBase] AS [CRMOP]
			INNER JOIN [Billing].[OrderPositions] AS [OP] ON [CRMOP].[Dg_orderpositionId] = [OP].[ReplicationCode] AND [OP].[Id] = @Id;
		
		
		UPDATE [CRMOP]
			SET 
				[Dg_amount] = [OP].[Amount]
			   ,[Dg_discountinpercent] = [OP].[DiscountPercent]	
			   ,[Dg_CategoryRate] = [OP].[CategoryRate]
			   ,[Dg_order]    = (SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OP].[OrderId])
			   ,[Dg_position] = (SELECT [P].[ReplicationCode] FROM [Billing].[Positions] AS [P] 
					INNER JOIN [Billing].[PricePositions] [PP] ON [PP].[PositionId] = [P].[Id] AND [PP].[Id] = [OP].[PricePositionId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_orderpositionExtensionBase] AS [CRMOP]
			INNER JOIN [Billing].[OrderPositions] AS [OP] ON [CRMOP].[Dg_orderpositionId] = [OP].[ReplicationCode] AND [OP].[Id] = @Id;		
	END;
	
	RETURN 1;


