-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateAccount]
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
	FROM [Billing].[Accounts] AS [TBL]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_accountBase] WHERE [Dg_accountId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_accountBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_accountId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
		   ,[OwningUser])
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[AC].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [AC].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[AC].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[AC].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [AC].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [AC].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
            @OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[Accounts] AS [AC]
		WHERE [AC].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_accountExtensionBase]
			([Dg_balance]
		    ,[Dg_branchofficeorganizationunit]
			,[Dg_legalperson]
			,[Dg_accountId]
            )
		SELECT
			 [AC].[Balance]
			,(SELECT [BOOU].ReplicationCode FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].Id = [AC].[BranchOfficeOrganizationUnitId])
			,(SELECT [LP].ReplicationCode FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].Id = [AC].[LegalPersonId])
			,[AC].[ReplicationCode]
		FROM [Billing].[Accounts] AS [AC]
		WHERE [AC].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [AC].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [AC].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [AC].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [AC].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
              ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_accountBase] AS [CRMAC]
			INNER JOIN [Billing].[Accounts] AS [AC] ON [CRMAC].[Dg_accountId] = [AC].[ReplicationCode] AND [AC].[Id] = @Id;
		

		UPDATE [CRMAC]
		SET 
			 [Dg_balance] = [AC].[Balance]
			,[Dg_branchofficeorganizationunit] = (SELECT [BOOU].ReplicationCode FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].Id = [AC].[BranchOfficeOrganizationUnitId])
			,[Dg_legalperson] = (SELECT [LP].ReplicationCode FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].Id = [AC].[LegalPersonId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_accountExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[Accounts] AS [AC] ON [CRMAC].[Dg_accountId] = [AC].[ReplicationCode] AND [AC].[Id] = @Id;
	END;
	
	RETURN 1;


