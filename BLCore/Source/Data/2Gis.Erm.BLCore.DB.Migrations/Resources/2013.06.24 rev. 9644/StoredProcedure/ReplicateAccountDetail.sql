-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateAccountDetail]
	@ID bigint = NULL
WITH EXECUTE AS CALLER
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
	FROM [Billing].[AccountDetails] AS [TBL]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_accountdetailBase] WHERE [Dg_accountdetailId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_accountdetailBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_accountdetailId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
		   ,[OwningUser]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[AD].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [AD].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[AD].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[AD].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			@OwnerUserId,-- <OwningBusinessUnit, uniqueidentifier,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [AD].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [AD].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL				-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[AccountDetails] AS [AD]
		WHERE [AD].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_accountdetailExtensionBase]
			([Dg_accountdetailId]
           ,[Dg_description]
           ,[Dg_amount]
           ,[Dg_comment]
           ,[Dg_transactiondate]
           ,[Dg_account]
           ,[Dg_operationtype])
		SELECT
			[AD].[ReplicationCode]
			,[AD].[Description]
			,[AD].[Amount]
			,[AD].[Comment]
			,[AD].[TransactionDate]
			,(SELECT [AC].ReplicationCode FROM [Billing].[Accounts] AS [AC] WHERE [AC].Id = [AD].[AccountId])
			,(SELECT [OT].ReplicationCode FROM [Billing].[OperationTypes] AS [OT] WHERE [OT].Id = [AD].[OperationTypeId])
		FROM [Billing].[AccountDetails] AS [AD]
		WHERE [AD].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [AD].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [AD].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [AD].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [AD].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
              ,[OwningUser] = @OwnerUserId
		FROM [DoubleGis_MSCRM].[dbo].[Dg_accountdetailBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
		

		UPDATE [CRMAC]
		SET 
			 [Dg_description] = [AD].[Description]
			,[Dg_amount] = [AD].[Amount]
			,[Dg_comment] = [AD].[Comment]
			,[Dg_transactiondate] = [AD].[TransactionDate]
			,[Dg_account] = (SELECT [AC].ReplicationCode FROM [Billing].[Accounts] AS [AC] WHERE [AC].Id = [AD].[AccountId])
			,[Dg_operationtype] = (SELECT [OT].ReplicationCode FROM [Billing].[OperationTypes] AS [OT] WHERE [OT].Id = [AD].[OperationTypeId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_accountdetailExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
	END;
	
	RETURN 1;


