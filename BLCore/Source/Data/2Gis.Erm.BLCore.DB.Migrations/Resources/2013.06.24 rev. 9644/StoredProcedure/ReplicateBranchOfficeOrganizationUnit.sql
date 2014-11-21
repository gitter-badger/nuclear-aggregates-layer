-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateBranchOfficeOrganizationUnit]
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
	FROM [Billing].[BranchOfficeOrganizationUnits] AS [TBL]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitBase] WHERE [Dg_branchoffice_organizationunitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_branchoffice_organizationunitId]
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
			[BOOU].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [BOOU].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[BOOU].[ReplicationCode],	-- <Dg_branchoffice_organizationunitId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[BOOU].[ModifiedOn],		-- <ModifiedOn, datetime,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,	-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [BOOU].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [BOOU].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL,						-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId				-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU]
		WHERE [BOOU].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitExtensionBase]
           ([dg_branchoffice_organizationunitId]
           ,[dg_name]
		   ,[dg_kpp]
           ,[dg_branchoffice]
           ,[dg_organizationunit]
		   ,[dg_paymentessentialelements])
		SELECT
			 [BOOU].[ReplicationCode]
			,[BOOU].[ShortLegalName]	-- <Dg_name, NVARCHAR(100)>
			,[BOOU].[Kpp]				-- <Dg_kpp, NVARCHAR(15)>
										-- <dg_branchoffice, NVARCHAR(100),>
			,(SELECT [BO].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BO] WHERE [BO].[Id] = [BOOU].[BranchOfficeId])
										-- <dg_organizationunit, NVARCHAR(100),>
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [BOOU].[OrganizationUnitId])
			,[BOOU].[PaymentEssentialElements]
		FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU]
		WHERE [BOOU].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMBOOU]
			SET 
			  [DeletionStateCode] = CASE WHEN [BOOU].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [BOOU].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [BOOU].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [BOOU].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitBase] AS [CRMBOOU]
			INNER JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [CRMBOOU].[Dg_branchoffice_organizationunitId] = [BOOU].[ReplicationCode] AND [BOOU].[Id] = @Id;
		
		
		UPDATE [CRMBOU]
		   SET 
			   [dg_name] = [BOOU].[ShortLegalName]
			  ,[dg_kpp]  = [BOOU].[KPP]
			  ,[dg_branchoffice] = (SELECT [BO].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BO] WHERE [BO].[Id] = [BOOU].[BranchOfficeId])
			  ,[dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [BOOU].[OrganizationUnitId])
			  ,[dg_paymentessentialelements] = [BOOU].[PaymentEssentialElements]
		FROM [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitExtensionBase] AS [CRMBOU]
			INNER JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [CRMBOU].[Dg_branchoffice_organizationunitId] = [BOOU].[ReplicationCode] AND [BOOU].[Id] = @Id;		
	END;
	
	RETURN 1;


