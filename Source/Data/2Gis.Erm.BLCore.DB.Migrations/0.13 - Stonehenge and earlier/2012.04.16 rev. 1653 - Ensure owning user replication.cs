using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	// ReSharper disable InconsistentNaming
	[Migration(1653, "Гарантировать репликацию куратора для всех сущностей с типом собествености пользователь")]
	public sealed class Migration_1653 : TransactedMigration
	{
		#region SP templates

		private const String UpdateReplicateAccountTemplate =
			@"
BEGIN
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
	
	RETURN 1;
END;";

		private const String UpdateReplicateAccountDetailTemplate =
			@"
BEGIN    
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
			  ,[statecode]  = CASE WHEN [AD].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [AD].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
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
	
	RETURN 1;
END;";

		private const String UpdateReplicateBargainTemplate =
			@"
BEGIN
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
	
	RETURN 1;
END;";

		private const String UpdateReplicateCurrencyTemplate =
			@"
BEGIN
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
	
	RETURN 1;
END;";

		private const String UpdateReplicateOperationTypeTemplate =
			@"
BEGIN    
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
			  ,[statecode]  = CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[Dg_operationtypeBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
		

		UPDATE [CRMAC]
			SET 
			 [Dg_name] = [OT].[Name]
			,[Dg_IsPlus] = [OT].[IsPlus]
		FROM [{0}].[dbo].[Dg_operationtypeExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
	END;
	
	RETURN 1;
END;";

		private const String UpdateReplicatePositionTemplate =
			@"
BEGIN    
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
	
	RETURN 1;
END;";

		#endregion

		private readonly IEnumerable<Tuple<SchemaQualifiedObjectName, String>> UpdateStoredProcedures = new List<Tuple<SchemaQualifiedObjectName, String>> 
		{
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicateAccount, UpdateReplicateAccountTemplate),
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicateAccountDetail, UpdateReplicateAccountDetailTemplate),
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicateBargain, UpdateReplicateBargainTemplate),
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicateCurrency, UpdateReplicateCurrencyTemplate),
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicateOperationType, UpdateReplicateOperationTypeTemplate),
			new Tuple<SchemaQualifiedObjectName, String>(ErmStoredProcedures.ReplicatePosition, UpdateReplicatePositionTemplate)
		};

		protected override void ApplyOverride(IMigrationContext context)
		{
			foreach(var sp in UpdateStoredProcedures)
			{
				var procedure = context.Database.StoredProcedures[sp.Item1.Name, sp.Item1.Schema];
				if (procedure == null)
				{
					throw new InvalidOperationException("Specified db doesn't contains procedure: " + sp.Item1);
				}

				procedure.TextBody = String.Format(sp.Item2, context.CrmDatabaseName);
				procedure.Alter();
			}
		}
	}
}
