-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   11.09.2014, a.tukaev: выпиливаем like при поиске пользователя по account
ALTER PROCEDURE [Billing].[ReplicateCurrency]
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
	FROM [Billing].[Currencies] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	WHERE [ErmUserAccount] = @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	    WHERE [ErmUserAccount] = @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	    WHERE [ErmUserAccount] = @ModifiedByUserDomainName;


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



