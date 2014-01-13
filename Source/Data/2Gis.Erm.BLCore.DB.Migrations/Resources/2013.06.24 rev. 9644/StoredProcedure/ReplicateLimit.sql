-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateLimit]
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

	DECLARE @InspectorUserDomainName NVARCHAR(250);
	DECLARE @InspectorUserId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @InspectorUserDomainName = [I].[Account],
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[Limits] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
        LEFT OUTER JOIN [Security].[Users] AS [I] ON [I].[Id] = [TBL].[InspectorCode]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

	-- get inspector user CRM UserId
    IF (@InspectorUserDomainName IS NOT NULL)
	    SELECT @InspectorUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @InspectorUserDomainName;

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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_limitBase] WHERE [Dg_limitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_limitBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_limitId]
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
			[LM].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [LM].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[LM].[ReplicationCode],	-- <Dg_limitId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[LM].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [LM].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [LM].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[Limits] AS [LM]
		WHERE [LM].[Id] = @Id;
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_limitExtensionBase]
           ([Dg_limitId],
		    [Dg_closedate],
			[Dg_amount],
			[Dg_status],
			[Dg_startperioddate],
			[Dg_endperioddate],
			[Dg_legalperson],
			[Dg_branchoffice_organizationunit],
			[Dg_comment],
			[Dg_inspectorid])
		SELECT
			 [LM].[ReplicationCode],
			 [LM].[CloseDate],
			 [LM].[Amount],
			 [LM].[Status],
			 [LM].[StartPeriodDate],
			 [LM].[EndPeriodDate],
			 (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP]
											INNER JOIN [Billing].[Accounts] AS [AC] ON [AC].[LegalPersonId] = [LP].[Id] 
											WHERE [AC].[Id] = [LM].[AccountId]),
			 (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] 
											INNER JOIN [Billing].[Accounts] AS [AC] ON [AC].[BranchOfficeOrganizationUnitId] = [BOOU].[Id]
											WHERE [AC].[Id] = [LM].[AccountId]),
			 [LM].[Comment],
			 @InspectorUserId
		FROM [Billing].[Limits] AS [LM]
		WHERE [LM].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMLM]
			SET 
			  [DeletionStateCode] = CASE WHEN [LM].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [LM].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [LM].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [LM].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_limitBase] AS [CRMLM]
			INNER JOIN [Billing].[Limits] AS [LM] ON [CRMLM].[Dg_limitId] = [LM].[ReplicationCode] AND [LM].[Id] = @Id;
		
		
		UPDATE [CRMLM]
			SET 
				[Dg_closedate] = [LM].[CloseDate],
				[Dg_amount] = [LM].[Amount],
				[Dg_status] = [LM].[Status],
				[Dg_startperioddate] = [LM].[StartPeriodDate],
				[Dg_endperioddate] = [LM].[EndPeriodDate],
				[Dg_legalperson] = (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP]
											INNER JOIN [Billing].[Accounts] AS [AC] ON [AC].[LegalPersonId] = [LP].[Id] 
											WHERE [AC].[Id] = [LM].[AccountId]),
				[Dg_branchoffice_organizationunit] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] 
											INNER JOIN [Billing].[Accounts] AS [AC] ON [AC].[BranchOfficeOrganizationUnitId] = [BOOU].[Id]
											WHERE [AC].[Id] = [LM].[AccountId]),
				[Dg_comment] = [LM].[Comment],
				[Dg_inspectorid] = @InspectorUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_limitExtensionBase] AS [CRMLM]
			INNER JOIN [Billing].[Limits] AS [LM] ON [CRMLM].[Dg_limitId] = [LM].[ReplicationCode] AND [LM].[Id] = @Id;		
	END;
	
	RETURN 1;



