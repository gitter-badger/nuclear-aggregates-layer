-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateLegalPerson]
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
	FROM [Billing].[LegalPersons] AS [TBL]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_legalpersonBase] WHERE Dg_legalpersonId = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_legalpersonBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_legalpersonId]
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
			@CreatedByUserId,	-- <CreatedBy, uniqueidentifier,>
			LP.[CreatedOn],		-- <CreatedOn, datetime,>
			CASE WHEN [LP].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			LP.[ReplicationCode],			-- <Dg_legalpersonId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			LP.[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [LP].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [LP].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[LegalPersons] AS [LP]
		WHERE [LP].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_legalpersonExtensionBase]
           ([dg_inn]
           ,[dg_kpp]
           ,[dg_legalpersonId]
           ,[dg_name]
		   ,[dg_legalname]
           ,[dg_account_legalperson]
		   ,[dg_passportseries]
		   ,[dg_legaladdress]
		   ,[dg_legalpersontype]
		   ,[dg_passportnumber])
		SELECT
			 LP.[Inn]		-- <Dg_inn, NVARCHAR(100),>
			,LP.[Kpp]   	-- <Dg_kpp, NVARCHAR(100),>
			,LP.[ReplicationCode]	-- <Dg_legalpersonId, uniqueidentifier,>
			,LP.[ShortName]			-- <Dg_name, NVARCHAR(100),>
			,LP.[LegalName]			-- <dg_account_legalperson, uniqueidentifier,>
			,(SELECT [Cl].[ReplicationCode] FROM [Billing].[Clients] AS [Cl] WHERE [Cl].[Id] = [LP].[ClientId])
			,LP.[PassportSeries]			-- <dg_passportseries, NVARCHAR(4),>
			,LP.[LegalAddress]
			,LP.[LegalPersonTypeEnum] + 1	-- енум в ерм и црм рассинхронизован. В ерм значимые значения идут с нуля. Нужно плюсовать для согласованности енумов 
			,LP.[PassportNumber]			-- <dg_passportnumber, NVARCHAR(7),>
		FROM [Billing].[LegalPersons] AS [LP]
		WHERE [LP].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMLP]
			SET 
			  [DeletionStateCode] = CASE WHEN [LP].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [LP].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [LP].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [LP].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_legalpersonBase] AS [CRMLP]
			INNER JOIN [Billing].[LegalPersons] AS [LP] ON [CRMLP].[Dg_legalpersonId] = [LP].[ReplicationCode] AND [LP].[Id] = @Id;
		
		UPDATE [CRMLP]
		SET 
			   [Dg_inn]  = [LP].[Inn]
			  ,[Dg_kpp]  = [LP].[Kpp]
			  ,[Dg_name] = [LP].[ShortName]
			  ,[Dg_legalname] = [LP].[LegalName]
			  ,[dg_account_legalperson] = (SELECT [Cl].[ReplicationCode] FROM [Billing].[Clients] AS [Cl] WHERE [Cl].[Id] = [LP].[ClientId])
			  ,[dg_passportseries] = LP.[PassportSeries]
			  ,[dg_passportnumber] = LP.[PassportNumber]
			  ,[dg_legaladdress] = LP.[LegalAddress]
			  ,[dg_legalpersontype] = LP.[LegalPersonTypeEnum] + 1
		FROM [DoubleGis_MSCRM].[dbo].[Dg_legalpersonExtensionBase] AS [CRMLP]
			INNER JOIN [Billing].[LegalPersons] AS [LP] ON [CRMLP].[Dg_legalpersonId] = [LP].[ReplicationCode] AND [LP].[Id] = @Id;	
	END;

    RETURN 1;


