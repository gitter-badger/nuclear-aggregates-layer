using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2327, "Обновление хранимых процедур репликации сущностей для обновления поля OwningBusinessUnit. +" +
                           "Сущности: Account, AccountDetail, OperationType, Bargain, Client, Contact, Currency, Deal, Position")]
    public class Migration2327 : TransactedMigration
    {
        #region SQL statement

        private const string CreateReplicateAccountSPTemplate = @"
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_accountBase] WHERE [Dg_accountId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_accountBase]
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
		
		
		INSERT INTO [{0}].[dbo].[Dg_accountExtensionBase]
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
		FROM [{0}].[dbo].[Dg_accountBase] AS [CRMAC]
			INNER JOIN [Billing].[Accounts] AS [AC] ON [CRMAC].[Dg_accountId] = [AC].[ReplicationCode] AND [AC].[Id] = @Id;
		

		UPDATE [CRMAC]
		SET 
			 [Dg_balance] = [AC].[Balance]
			,[Dg_branchofficeorganizationunit] = (SELECT [BOOU].ReplicationCode FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].Id = [AC].[BranchOfficeOrganizationUnitId])
			,[Dg_legalperson] = (SELECT [LP].ReplicationCode FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].Id = [AC].[LegalPersonId])
		FROM [{0}].[dbo].[Dg_accountExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[Accounts] AS [AC] ON [CRMAC].[Dg_accountId] = [AC].[ReplicationCode] AND [AC].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const String CreateReplicateAccountDetailSPTemplate = @"
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_accountdetailBase] WHERE [Dg_accountdetailId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_accountdetailBase]
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
           ,[UTCConversionTimeZoneCode]
		   )
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
		
		
		INSERT INTO [{0}].[dbo].[Dg_accountdetailExtensionBase]
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
		FROM [{0}].[dbo].[Dg_accountdetailBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
		

		UPDATE [CRMAC]
		SET 
			 [Dg_description] = [AD].[Description]
			,[Dg_amount] = [AD].[Amount]
			,[Dg_comment] = [AD].[Comment]
			,[Dg_transactiondate] = [AD].[TransactionDate]
			,[Dg_account] = (SELECT [AC].ReplicationCode FROM [Billing].[Accounts] AS [AC] WHERE [AC].Id = [AD].[AccountId])
			,[Dg_operationtype] = (SELECT [OT].ReplicationCode FROM [Billing].[OperationTypes] AS [OT] WHERE [OT].Id = [AD].[OperationTypeId])
		FROM [{0}].[dbo].[Dg_accountdetailExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const String CreateReplicateOperationTypeSPTemplate = @"
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
	FROM [Billing].[OperationTypes] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_operationtypeBase] WHERE [Dg_operationtypeId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_operationtypeBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_operationtypeId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
		   ,[OwningUser]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
		   )
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[OT].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OT].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[OT].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			@OwnerUserId,
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL				-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_operationtypeExtensionBase]
			([Dg_operationtypeId]
           ,[Dg_name]
           ,[Dg_IsPlus])
		SELECT 
			 [OT].[ReplicationCode]
			,[OT].[Name]
			,[OT].[IsPlus]
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OT].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
              ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_operationtypeBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
		

		UPDATE [CRMAC]
			SET 
			 [Dg_name] = [OT].[Name]
			,[Dg_IsPlus] = [OT].[IsPlus]
		FROM [{0}].[dbo].[Dg_operationtypeExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const string CreateReplicateBargainSPTemplate = @"
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
    FROM [Billing].[Bargains] AS [TBL]
	    LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

    -- get owner user CRM UserId
    SELECT 
	    @OwnerUserId = [SystemUserId],
	    @OwnerUserBusinessUnitId = [BusinessUnitId]
    FROM [{0}].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


    IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_bargainBase] WHERE [Dg_bargainId] = @CrmId)
    --------------------------------------------------------------------------
    -- there is no such a record in the CRM database => insert a brand new one
    --------------------------------------------------------------------------
    BEGIN

	    INSERT INTO [{0}].[dbo].[Dg_bargainBase]
            ([CreatedBy]
            ,[CreatedOn]
            ,[DeletionStateCode]
            ,[Dg_bargainId]
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
		    [BG].[CreatedOn],	-- <CreatedOn, datetime,>
		    CASE WHEN [BG].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
		    [BG].[ReplicationCode],			-- <Dg_bargainId, uniqueidentifier,>
		    NULL,				-- <ImportSequenceNumber, int,>
		    ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
		    [BG].[ModifiedOn],	-- <ModifiedOn, datetime,>
		    NULL,				-- <OverriddenCreatedOn, datetime,>
		    @OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
		    CASE WHEN [BG].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
		    CASE WHEN [BG].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
		    NULL,				-- <TimeZoneRuleVersionNumber, int,>
		    NULL,				-- <UTCConversionTimeZoneCode, int,>
		    @OwnerUserId		-- <OwningUser, uniqueidentifier,>
	    FROM [Billing].[Bargains] AS [BG]
	    WHERE [BG].[Id] = @Id;
		
		
	    INSERT INTO [{0}].[dbo].[Dg_bargainExtensionBase]
            ([Dg_number]
            ,[Dg_comment]
            ,[Dg_signedon]
            ,[Dg_legalperson]
            ,[Dg_branchoffice]
            ,[Dg_bargainId]
		    ,[Dg_closedon])
	    SELECT
			    [BG].[Number]				-- <Dg_name, NVARCHAR(100),>
		    ,[BG].[Comment]
		    ,[BG].[SignedOn]
		    ,(SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [BG].[CustomerLegalPersonId])
		    ,(SELECT [BO].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BO] WHERE [BO].[Id] = [BG].[ExecutorBranchOfficeId])
		    ,[BG].[ReplicationCode]		-- <Dg_bargainId, uniqueidentifier,>
		    ,[BG].[ClosedOn]
	    FROM [Billing].[Bargains] AS [BG]
	    WHERE [BG].[Id] = @Id;

    END

    -------------------------------------------------------
    -- there is already saved record => update existing one
    -------------------------------------------------------
    ELSE
    BEGIN
		
	    UPDATE [CRMBG]
		    SET 
			    [DeletionStateCode] = CASE WHEN [BG].[IsDeleted] = 1 THEN 2 ELSE 0 END
			    --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			    ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			    ,[ModifiedOn] = [BG].[ModifiedOn]
			    --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
				,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			    ,[statecode]  = CASE WHEN [BG].[IsActive] = 1 THEN 0 ELSE 1 END
			    ,[statuscode] = CASE WHEN [BG].[IsActive] = 1 THEN 1 ELSE 2 END
			    --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			    --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
                ,[OwningUser] = @OwnerUserId
	    FROM [{0}].[dbo].[Dg_bargainBase] AS [CRMBG]
		    INNER JOIN [Billing].[Bargains] AS [BG] ON [CRMBG].[Dg_bargainId] = [BG].[ReplicationCode] AND [BG].[Id] = @Id;
		
	    UPDATE [CRMBG]
	    SET 
		        [Dg_number] = [BG].[Number],
		        [Dg_comment] = [BG].[Comment],
		        [Dg_signedon] = [BG].[SignedOn],
		        [Dg_legalperson] = (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [BG].[CustomerLegalPersonId]),
		        [Dg_branchoffice] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BOOU] WHERE [BOOU].[Id] = [BG].[ExecutorBranchOfficeId]),
			    [Dg_closedon] = [BG].[ClosedOn]
	    FROM [{0}].[dbo].[Dg_bargainExtensionBase] AS [CRMBG]
		    INNER JOIN [Billing].[Bargains] AS [BG] ON [CRMBG].[Dg_bargainId] = [BG].[ReplicationCode] AND [BG].[Id] = @Id;
    END;
	
    RETURN 1;";

        private const string CreateReplicateClientSPTemplate = @"
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
	FROM [Billing].[Clients] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[AccountBase] WHERE [AccountId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[AccountBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[AccountId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OverriddenCreatedOn]
           ,[OwningBusinessUnit]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
           ,[OwningUser]
		   ,[Name]
		   ,[Telephone1]
		   ,[EMailAddress1])
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[CL].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [CL].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[CL].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[CL].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [CL].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [CL].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId,		-- <OwningUser, uniqueidentifier,>
			[CL].Name,
			[CL].MainPhoneNumber,
			[CL].Email
		FROM [Billing].[Clients] AS [CL]
		WHERE [CL].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[AccountExtensionBase]
           ( [dg_mainaddress]
		    ,[dg_mainfirm]
			,[dg_lastqualifytime]
			,[dg_lastdisqualifytime]
			,[dg_source]
			,[dg_territory]
			,[accountid]
			,[dg_PromisingScore]
            )
		SELECT
			 [CL].[MainAddress]
			,(SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [CL].MainFirmId)
			,[CL].[LastQualifyTime]
			,[CL].[LastDisqualifyTime]
			,[CL].[InformationSource]
			,(SELECT [TR].ReplicationCode FROM [BusinessDirectory].Territories AS [TR] WHERE [TR].Id = [CL].TerritoryId)
			,[CL].[ReplicationCode]		-- <AccountId, uniqueidentifier,>
			,[CL].[PromisingValue]
		FROM [Billing].[Clients] AS [CL]
		WHERE [CL].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMCL]
			SET 
			  [DeletionStateCode] = CASE WHEN [CL].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [CL].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [CL].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [CL].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,Name = [CL].Name
			  ,[Telephone1] = [CL].MainPhoneNumber
			  ,[EMailAddress1] = [CL].Email
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[AccountBase] AS [CRMCL]
			INNER JOIN [Billing].[Clients] AS [CL] ON [CRMCL].[AccountId] = [CL].[ReplicationCode] AND [CL].[Id] = @Id;
		
		UPDATE [CRMCL]
		SET 
			 [dg_mainaddress] = [CL].[MainAddress]
			,[dg_mainfirm] = (SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [CL].MainFirmId)
			,[dg_lastqualifytime] = [CL].[LastQualifyTime]
			,[dg_lastdisqualifytime] = [CL].[LastDisqualifyTime]
			,[dg_source] = [CL].[InformationSource]
			,[dg_territory] = (SELECT [TR].ReplicationCode FROM [BusinessDirectory].Territories AS [TR] WHERE [TR].Id = [CL].TerritoryId)
			,[accountid] = [CL].[ReplicationCode]
			,[dg_PromisingScore] = [CL].[PromisingValue]
		FROM [{0}].[dbo].[AccountExtensionBase] AS [CRMCL]
			INNER JOIN [Billing].[Clients] AS [CL] ON [CRMCL].[AccountId] = [CL].[ReplicationCode] AND [CL].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const string CreateReplicateContactSPTemplate = @"
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
	FROM [Billing].[Contacts] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[ContactBase] WHERE [ContactId] = @CrmId)
	BEGIN

		INSERT INTO [{0}].[dbo].[ContactBase]
           ([CreatedBy],
            [CreatedOn],
            [DeletionStateCode],
            [ContactId],
            [ImportSequenceNumber],
            [ModifiedBy],
            [ModifiedOn],
            [OverriddenCreatedOn],
            [OwningBusinessUnit],
		    [OwningUser],
            [statecode],
            [statuscode],

			[FullName],
			[FirstName],
			[MiddleName],
			[LastName],
			[Salutation],
			[GenderCode],
			[FamilyStatusCode],
			[Telephone1], -- рабочий (основной) телефон
			[Telephone2], -- домашний телефон
			[Telephone3], -- телефон 3
			[MobilePhone],
			[Fax],
			[EMailAddress1], -- рабочая электронная почта
			[EMailAddress2], -- домашняя электронная почта
			[WebSiteUrl],
			[AccountId],
			[JobTitle],
			[AccountRoleCode],
			[Department],
			[Description],
			[BirthDate]
			)

		SELECT
			@CreatedByUserId,
			[Contacts].[CreatedOn],
			CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[Contacts].[ReplicationCode],
			NULL,
			ISNULL(@ModifiedByUserId, @CreatedByUserId),
			[Contacts].[ModifiedOn],
			NULL,
			@OwnerUserBusinessUnitId,
			@OwnerUserId,
			CASE WHEN [Contacts].[IsActive] = 1 THEN 0 ELSE 1 END,
			CASE WHEN [Contacts].[IsActive] = 1 THEN 1 ELSE 2 END,

			[Contacts].[FullName],
			[Contacts].[FirstName],
			[Contacts].[MiddleName],
			[Contacts].[LastName],
			[Contacts].[Salutation],
			[Contacts].[GenderCode],
			[Contacts].[FamilyStatusCode],
			[Contacts].[MainPhoneNumber],
			[Contacts].[HomePhoneNumber],
			[Contacts].[AdditionalPhoneNumber],
			[Contacts].[MobilePhoneNumber],
			[Contacts].[Fax],
			[Contacts].[WorkEmail],
			[Contacts].[HomeEmail],
			[Contacts].[Website],
			(SELECT [Clients].[ReplicationCode] FROM [Billing].[Clients] WHERE [Clients].Id = [Contacts].[ClientId]),
			[Contacts].[JobTitle],
			[Contacts].[AccountRole],
			[Contacts].[Department],
			[Contacts].[Comment],
			[BirthDate]

		FROM [Billing].[Contacts]
		WHERE [Contacts].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[ContactExtensionBase]
           ([ContactId],
		    [Dg_workaddress],
		    [Dg_homeaddress],
			[Dg_imidentifier],
			[Dg_isfired]
            )
		SELECT
			[ReplicationCode],
			[WorkAddress],
			[HomeAddress],
			[ImIdentifier],
			[IsFired]
		FROM [Billing].[Contacts]
		WHERE [Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [ContactBase]
			SET
			[DeletionStateCode] = CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId),
			[ModifiedOn] = [Contacts].[ModifiedOn],
			[statecode]  = CASE WHEN [Contacts].[IsActive] = 1 THEN 0 ELSE 1 END,
			[statuscode] = CASE WHEN [Contacts].[IsActive] = 1 THEN 1 ELSE 2 END,
			[OwningBusinessUnit] = @OwnerUserBusinessUnitId,
			[OwningUser] = @OwnerUserId,

			[FullName] = [Contacts].[FullName],
			[FirstName] = [Contacts].[FirstName],
			[MiddleName] = [Contacts].[MiddleName],
			[LastName] = [Contacts].[LastName],
			[Salutation] = [Contacts].[Salutation],
			[GenderCode] = [Contacts].[GenderCode],
			[FamilyStatusCode] = [Contacts].[FamilyStatusCode],
			[Telephone1] = [Contacts].[MainPhoneNumber],
			[Telephone2] = [Contacts].[HomePhoneNumber],
			[Telephone3] = [Contacts].[AdditionalPhoneNumber],
			[MobilePhone] = [Contacts].[MobilePhoneNumber],
			[Fax] = [Contacts].[Fax],
			[EMailAddress1] = [Contacts].[WorkEmail],
			[EMailAddress2] = [Contacts].[HomeEmail],
			[WebSiteUrl] = [Contacts].[Website],
			[AccountId] = (SELECT [Clients].[ReplicationCode] FROM [Billing].[Clients] WHERE [Clients].Id = [Contacts].[ClientId]),
			[JobTitle] = [Contacts].[JobTitle],
			[AccountRoleCode] = [Contacts].[AccountRole],
			[Department] = [Contacts].[Department],
			[Description] = [Contacts].[Comment],
			[BirthDate] = [Contacts].[BirthDate]

		FROM [{0}].[dbo].[ContactBase] AS [ContactBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] = @Id;
		
		UPDATE [ContactExtensionBase]
		SET 
			[ContactId] = [Contacts].[ReplicationCode],
			[Dg_workaddress] = [Contacts].[WorkAddress],
			[Dg_homeaddress] = [Contacts].[HomeAddress],
			[Dg_imidentifier] = [Contacts].[ImIdentifier],
			[Dg_isfired] = [Contacts].[IsFired]
		FROM [{0}].[dbo].[ContactExtensionBase] AS [ContactExtensionBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactExtensionBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] = @Id;
	END;
	
	RETURN 1";

        private const string CreateReplicateCurrencySPTemplate = @"
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
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_currencyBase] WHERE [Dg_currencyId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_currencyBase]
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
		
		
		INSERT INTO [{0}].[dbo].[Dg_currencyExtensionBase]
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
		FROM [{0}].[dbo].[Dg_currencyBase] AS [CRMCR]
			INNER JOIN [Billing].[Currencies] AS [CR] ON [CRMCR].[Dg_currencyId] = [CR].[ReplicationCode] AND [CR].[Id] = @Id;
		
		UPDATE [CRMCR]
		SET 
		      [Dg_name] = [CR].[Name]
			 ,[Dg_currencysymbol] = [CR].[Symbol]
		     ,[Dg_isbase] = [CR].[IsBase]
		FROM [{0}].[dbo].[Dg_currencyExtensionBase] AS [CRMCR]
			INNER JOIN [Billing].[Currencies] AS [CR] ON [CRMCR].[Dg_currencyId] = [CR].[ReplicationCode] AND [CR].[Id] = @Id;
	END;
	
	RETURN 1";

        private const string CreateReplicateDealSPTemplate = @"
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
	FROM [Billing].[Deals] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;

	-- get StateCode and StatusCode for opportunity:
	-- state	status
	-- 0(open) 1(in progress)
	-- 1(open) 2(on hold)
	-- 1(won)  3(won)
	-- 2(lost) 4(cancell)
	-- 2(lost) 5(out-sold)

	DECLARE @StateCode INT;
	DECLARE @StatusCode INT;
	DECLARE @ActivityId UNIQUEIDENTIFIER;

	SELECT @StateCode = CASE
		WHEN ([D].[IsActive] = 1) THEN 0
		WHEN ([D].[IsActive] = 0 AND [DE].[ActualProfit] > 0) THEN 1
		WHEN ([D].[IsActive] = 0 AND [DE].[ActualProfit] = 0) THEN 2
		END 
	FROM [Billing].[Deals] AS [D]
	JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
	WHERE [DE].[Id] =  @Id;

	SET @StatusCode = CASE
		WHEN (@StateCode = 0) THEN 1
		WHEN (@StateCode = 1) THEN 3
		WHEN (@StateCode = 2) THEN 5
	END  

	--------------------------------------------------------------------------
	-- INSERT NEW
	--------------------------------------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[OpportunityBase] WHERE [OpportunityId] = @CrmId)
	BEGIN

		-- OpportunityBase
		INSERT INTO [{0}].[dbo].[OpportunityBase]
			([OpportunityId]
			,[CreatedOn]
			,[DeletionStateCode]
			,[CreatedBy]
			,[ModifiedBy]
			,[ModifiedOn]
			,[OwningBusinessUnit]
			,[statecode]
			,[statuscode]
			,[OwningUser]
			,[AccountId]
			,[ActualCloseDate]
			,[Name]
			)
		SELECT
			[D].[ReplicationCode]		-- [OpportunityId], uniqueidentifier
			,[D].[CreatedOn]			-- [CreatedOn], datetime
			,CASE WHEN [D].[IsDeleted] = 1 THEN 2 ELSE 0 END -- [DeletionStateCode], int
			,@CreatedByUserId			-- [CreatedBy], uniqueidentifier
			,ISNULL(@ModifiedByUserId, @CreatedByUserId) -- [ModifiedBy], uniqueidentifier
			,[D].[ModifiedOn]			-- [ModifiedOn], datetime
			,@OwnerUserBusinessUnitId	-- [OwningBusinessUnit], uniqueidentifier
			,@StateCode
			,@StatusCode
			,@OwnerUserId				-- [OwningUser], uniqueidentifier
			,(SELECT [CL].ReplicationCode FROM [Billing].Clients AS [CL] WHERE [CL].Id = [D].[ClientId]) -- [ContactId], uniqueidentifier
			,[D].[CloseDate]			-- [ActualCloseDate], datetime
			,[D].Name					-- [Name], NVARCHAR(300)
		FROM [Billing].[Deals] AS [D]
		WHERE [D].[Id] = @Id;
		
		-- OpportunityExtensionBase
		INSERT INTO [{0}].[dbo].[OpportunityExtensionBase]
			([OpportunityId]
			,[dg_actualprofit]
			,[dg_closereason]
			,[dg_closereasonother]
			,[dg_dealstage]
			,[dg_estimatedprofit]
			,[dg_firm]
			,[dg_isactive]
			,[dg_startreason]
			,[dg_currency])
		SELECT
			[D].ReplicationCode
			,[DE].ActualProfit
			,[D].CloseReason
			,[D].CloseReasonOther
			,[DE].DealStage
			,[DE].EstimatedProfit
			,(SELECT [FR].ReplicationCode FROM [BusinessDirectory].[Firms] AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[D].IsActive
			,[D].StartReason
			,(SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
		FROM [Billing].[Deals] AS [D]
		JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
		WHERE [D].[Id] = @Id;
	END

	-------------------------------------------------------
	-- UPDATE EXISTING
	-------------------------------------------------------
	ELSE
	BEGIN
		
		-- OpportunityBase
		UPDATE [{0}].[dbo].[OpportunityBase]
			SET 
			[DeletionStateCode] = CASE WHEN [D].[IsDeleted] = 1 THEN 2 ELSE 0 END
			,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			,[ModifiedOn] = [D].[ModifiedOn]
			,[statecode]  = @StateCode
			,[statuscode] = @StateCode
			,[OwningUser] = @OwnerUserId
			,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			,[AccountId] = (SELECT [CL].ReplicationCode FROM [Billing].Clients AS [CL] WHERE [CL].Id = [D].[ClientId])
			,[ActualCloseDate] = [D].[CloseDate]
			,[Name] = [D].Name
		FROM [{0}].[dbo].[OpportunityBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id;
		
		-- OpportunityExtensionBase
		UPDATE [{0}].[dbo].[OpportunityExtensionBase]
		   SET 
			[OpportunityId] = [D].ReplicationCode
			,[dg_actualprofit] = [DE].ActualProfit
			,[dg_closereason]  = [D].CloseReason
			,[dg_closereasonother] = [D].CloseReasonOther
			,[dg_dealstage] = [DE].DealStage
			,[dg_estimatedprofit] = [DE].EstimatedProfit
			,[dg_firm]    = (SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[dg_isactive] = [D].IsActive
			,[dg_startreason] = [D].StartReason
			,[dg_currency] = (SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
		FROM [{0}].[dbo].[OpportunityExtensionBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id
			INNER JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
	END;

	--------------------------------------------------------------------------
	-- CLOSED DEAL
	--------------------------------------------------------------------------

	-- trying to replicate closed deal if CloseDate is set
	IF EXISTS (SELECT 1 FROM [Billing].[Deals] WHERE [CloseDate] IS NOT NULL AND [Id] = @Id)
	BEGIN
		
	-- add new closed deal
	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[ActivityPointerBase]
					WHERE [RegardingObjectId] = @CrmId
						  AND [RegardingObjectTypeCode] = 3 -- 3 is opportunity object type code
						  AND [ActualEnd] = (SELECT [CloseDate] FROM [Billing].[Deals] WHERE [Id] = @Id))
	BEGIN
		SET @ActivityId = NEWID()

		-- ActivityPointerBase
		INSERT INTO [{0}].[dbo].[ActivityPointerBase]
				([ActivityId]
				,[ActualEnd]
				,[Description]
				,[OwningBusinessUnit]
				,[IsBilled]
				,[RegardingObjectIdName]
				,[StateCode]
				,[ModifiedOn]
				,[StatusCode]
				,[Subject]
				,[IsWorkflowCreated]
				,[CreatedBy]
				,[OwningUser]
				,[ModifiedBy]
				,[RegardingObjectIdDsc]
				,[RegardingObjectId]
				,[RegardingObjectTypeCode]
				,[DeletionStateCode]
				,[CreatedOn]
				,[TimeZoneRuleVersionNumber]
				,[ActivityTypeCode]
				)
			SELECT
				@ActivityId
				,[D].[CloseDate]
				,[D].[Comment]
				,@OwnerUserBusinessUnitId
				,0
				,[D].[Name]
				,@StateCode
				,[D].[ModifiedOn]
				,@StatusCode
				,[D].[Name]
				,0
				,@CreatedByUserId
				,@OwnerUserId
				,@ModifiedByUserId
				,0
				,[D].[ReplicationCode]
				,3 -- 3 is opportunity object type code
				,CASE WHEN [D].[IsDeleted] = 1 THEN 2 ELSE 0 END
				,[D].[CreatedOn]
				,0
				,4208 -- 4208 is opportunityclose activity type code
		FROM [Billing].[Deals] AS [D]
		WHERE [D].[Id] = @Id;

		-- OpportunityCloseBase
		INSERT INTO [{0}].[dbo].[OpportunityCloseBase]
				([ActivityId]
				,[TransactionCurrencyId]
				,[ExchangeRate]
				,[ActualRevenue_Base]
				,[ActualRevenue]
				)
			SELECT
				@ActivityId
				-- TODO: репликация требует указания TransactionCurrency, она никак не связана с нашей dg_currency, разобраться
				,(SELECT TOP 1 [TransactionCurrencyId] FROM [{0}].[dbo].[TransactionCurrencyBase])
				,1 -- echange rate is always 1
				,0
				,[DE].ActualProfit
		FROM [Billing].[Deals] AS [D]
		JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
		WHERE [D].[Id] = @Id;

		-- ActivityPartyBase - opprtunityclose-user relation
		INSERT INTO [{0}].[dbo].[ActivityPartyBase]
				([ActivityPartyId]
				,[ActivityId]
				,[PartyId]
				,[PartyObjectTypeCode]
				,[ParticipationTypeMask]
				)
			SELECT
				NEWID()
				,@ActivityId
				,@OwnerUserId
				,8 -- 8 is user object type code
				,9 -- compatibility with dynamics crm 3.0
			FROM [Billing].[Deals] AS [D]
			JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
		WHERE [D].[Id] = @Id;

		-- ActivityPartyBase - opprtunityclose-opprtunity relation
		INSERT INTO [{0}].[dbo].[ActivityPartyBase]
				([ActivityPartyId]
				,[ActivityId]
				,[PartyId]
				,[PartyObjectTypeCode]
				,[ParticipationTypeMask]
				)
			SELECT
				NEWID()
				,@ActivityId
				,[D].[ReplicationCode]
				,3 -- 3 is opprtunity object type code
				,8 -- compatibility with dynamics crm 3.0
			FROM [Billing].[Deals] AS [D]
			JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id
		WHERE [D].[Id] = @Id;

	END

	ELSE 

	-- update existing closed deal, very rare case, not tested, may contain bugs
	BEGIN
		-- ActivityPointerBase
		UPDATE [{0}].[dbo].[ActivityPointerBase]
		   SET [RegardingObjectIdName] = [D].[Name]
		FROM [{0}].[dbo].[ActivityPointerBase] AS [AP]
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [RegardingObjectId] AND [RegardingObjectTypeCode] = 3 AND [D].[Id] = @Id

		-- OpportunityCloseBase
		UPDATE [{0}].[dbo].[OpportunityCloseBase]
		   SET [ActualRevenue] = [DE].[ActualProfit]
		FROM [{0}].[dbo].[OpportunityCloseBase] AS [OC]
			INNER JOIN [{0}].[dbo].[ActivityPointerBase] as [AP] ON [AP].[ActivityId] = [OC].[ActivityId] AND [AP].[RegardingObjectTypeCode] = 3
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [AP].[RegardingObjectId] AND [D].[Id] = @Id
			INNER JOIN [Billing].[DealExtensions] AS [DE] ON [DE].Id = [D].Id

		-- ActivityPartyBase - opprtunityclose-user
		-- no updates needed

		-- ActivityPartyBase - opprtunityclose-opprtunity relation
		UPDATE [{0}].[dbo].[ActivityPartyBase]
		   SET [PartyIdName] = [D].[Name]
		FROM [{0}].[dbo].[ActivityPartyBase] AS [APB]
			INNER JOIN [{0}].[dbo].[ActivityPointerBase] as [AP] ON [AP].[ActivityId] = [APB].[PartyId] AND [APB].[PartyObjectTypeCode] = 3 AND [AP].[RegardingObjectTypeCode] = 3
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [AP].[RegardingObjectId] AND [D].[Id] = @Id

	END
	END

	RETURN 1;";

        private const string CreateReplicatePositionSPTemplate =@"
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
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_positionBase] WHERE [Dg_positionId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_positionBase]
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
		
		
		INSERT INTO [{0}].[dbo].[Dg_positionExtensionBase]
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
		FROM [{0}].[dbo].[Dg_positionBase] AS [CRMP]
			INNER JOIN [Billing].[Positions] AS [P] ON [CRMP].[Dg_positionId] = [P].[ReplicationCode] AND [P].[Id] = @Id;
		
		UPDATE [CRMP]
		SET 
		      [Dg_name] = [P].[Name]
		FROM [{0}].[dbo].[Dg_positionExtensionBase] AS [CRMP]
			INNER JOIN [Billing].[Positions] AS [P] ON [CRMP].[Dg_positionId] = [P].[ReplicationCode] AND [P].[Id] = @Id;
	END;
	
	RETURN 1;";

        #endregion

        private class StoredProcInfo
        {
            public SchemaQualifiedObjectName QualifiedName { get; private set; }
            public string TextBodyTemplate { get; private set; }

            public StoredProcInfo(SchemaQualifiedObjectName qualifiedName, string textBodyTemplate)
            {
                QualifiedName = qualifiedName;
                TextBodyTemplate = textBodyTemplate;
            }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateAccount, CreateReplicateAccountSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateAccountDetail, CreateReplicateAccountDetailSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateOperationType, CreateReplicateOperationTypeSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateBargain, CreateReplicateBargainSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateClient, CreateReplicateClientSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateContact, CreateReplicateContactSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateCurrency, CreateReplicateCurrencySPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicateDeal, CreateReplicateDealSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);

            storedProcInfo = new StoredProcInfo(ErmStoredProcedures.ReplicatePosition, CreateReplicatePositionSPTemplate);
            AlterOrCreateReplicationSP(context, storedProcInfo, context.CrmDatabaseName);
        }

        private static void AlterOrCreateReplicationSP(IMigrationContext context, StoredProcInfo storedProcInfo, string environmentNumberString)
        {
            var spTextBody = string.Format(storedProcInfo.TextBodyTemplate, environmentNumberString);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, storedProcInfo.QualifiedName, spTextBody);
        }
    }
}