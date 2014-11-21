using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2630, "Обновление ВСЕХ хранимых процедур репликации сущностей для корректного изменения поля ModifiedBy")]
    public class Migration2630 : TransactedMigration
    {
        #region SQL statement

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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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

        private const string CreateReplicateBranchOfficeSPTemplate = @"
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
	FROM [Billing].[BranchOffices] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_branchofficeBase] WHERE [Dg_branchofficeId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_branchofficeBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_branchofficeId]
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
			[BO].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [BO].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[BO].[ReplicationCode],			-- <Dg_branchofficeId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[BO].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [BO].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [BO].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId		-- <OwningUser, uniqueidentifier,>
		FROM [Billing].[BranchOffices] AS [BO]
		WHERE [BO].[Id] = @Id;
		
		INSERT INTO [{0}].[dbo].[Dg_branchofficeExtensionBase]
           ([Dg_branchofficeId]
           ,[Dg_INN]
           ,[Dg_legalname]
		   )
		SELECT
			 [BO].[ReplicationCode]
			,[BO].[Inn]		-- <Dg_inn, NVARCHAR(100),>
			,[BO].[Name]		-- <Dg_legalname, NVARCHAR(256),>
		FROM [Billing].[BranchOffices] AS [BO]
		WHERE [BO].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMBO]
			SET 
			  [DeletionStateCode] = CASE WHEN [BO].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [BO].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [BO].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [BO].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[Dg_branchofficeBase] AS [CRMBO]
			INNER JOIN [Billing].[BranchOffices] AS [BO] ON [CRMBO].[Dg_branchofficeId] = [BO].[ReplicationCode] AND [BO].[Id] = @Id;
		
		
		UPDATE [CRMBO]
			SET 
			   [Dg_INN] = [BO].[Inn]
			  ,[Dg_legalname] = [BO].[Name]
		FROM [{0}].[dbo].[Dg_branchofficeExtensionBase] AS [CRMBO]
			INNER JOIN [Billing].[BranchOffices] AS [BO] ON [CRMBO].[Dg_branchofficeId] = [BO].[ReplicationCode] AND [BO].[Id] = @Id;		
	END;
	
	RETURN 1;";

        private const string CreateReplicateBranchOfficeOrganizationUnitSPTemplate = @"
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_branchoffice_organizationunitBase] WHERE [Dg_branchoffice_organizationunitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_branchoffice_organizationunitBase]
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
		
		INSERT INTO [{0}].[dbo].[Dg_branchoffice_organizationunitExtensionBase]
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
		FROM [{0}].[dbo].[Dg_branchoffice_organizationunitBase] AS [CRMBOOU]
			INNER JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [CRMBOOU].[Dg_branchoffice_organizationunitId] = [BOOU].[ReplicationCode] AND [BOOU].[Id] = @Id;
		
		
		UPDATE [CRMBOU]
		   SET 
			   [dg_name] = [BOOU].[ShortLegalName]
			  ,[dg_kpp]  = [BOOU].[KPP]
			  ,[dg_branchoffice] = (SELECT [BO].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BO] WHERE [BO].[Id] = [BOOU].[BranchOfficeId])
			  ,[dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [BOOU].[OrganizationUnitId])
			  ,[dg_paymentessentialelements] = [BOOU].[PaymentEssentialElements]
		FROM [{0}].[dbo].[Dg_branchoffice_organizationunitExtensionBase] AS [CRMBOU]
			INNER JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [CRMBOU].[Dg_branchoffice_organizationunitId] = [BOOU].[ReplicationCode] AND [BOOU].[Id] = @Id;		
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
		    [Dg_workaddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
		    [Dg_homeaddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
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
			[Dg_workaddress] = [Contacts].[WorkAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_homeaddress] = [Contacts].[HomeAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_imidentifier] = [Contacts].[ImIdentifier],
			[Dg_isfired] = [Contacts].[IsFired]
		FROM [{0}].[dbo].[ContactExtensionBase] AS [ContactExtensionBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactExtensionBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] = @Id;
	END;
	
	RETURN 1;";

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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
	
	RETURN 1;";

        private const string CreateReplicateLegalPersonSPTemplate = @"
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_legalpersonBase] WHERE Dg_legalpersonId = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_legalpersonBase]
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
		
		INSERT INTO [{0}].[dbo].[Dg_legalpersonExtensionBase]
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
		FROM [{0}].[dbo].[Dg_legalpersonBase] AS [CRMLP]
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
		FROM [{0}].[dbo].[Dg_legalpersonExtensionBase] AS [CRMLP]
			INNER JOIN [Billing].[LegalPersons] AS [LP] ON [CRMLP].[Dg_legalpersonId] = [LP].[ReplicationCode] AND [LP].[Id] = @Id;	
	END;

    RETURN 1;";

        private const string CreateReplicateLimitSPTemplate = @"
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
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

	-- get inspector user CRM UserId
    IF (@InspectorUserDomainName IS NOT NULL)
	    SELECT @InspectorUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @InspectorUserDomainName;

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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_limitBase] WHERE [Dg_limitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_limitBase]
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
		
		INSERT INTO [{0}].[dbo].[Dg_limitExtensionBase]
           ([Dg_limitId],
		    [Dg_closedate],
			[Dg_amount],
			[Dg_status],
			[Dg_startperioddate],
			[Dg_endperioddate],
			[Dg_legalperson],
			[Dg_account],
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
			 (SELECT [CL].[ReplicationCode] FROM [Billing].[Clients] AS [CL] WHERE [CL].[Id] = [LM].[ClientId]),
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
		FROM [{0}].[dbo].[Dg_limitBase] AS [CRMLM]
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
				[Dg_account] = (SELECT [CL].[ReplicationCode] FROM [Billing].[Clients] AS [CL] WHERE [CL].[Id] = [LM].[ClientId]),
				[Dg_branchoffice_organizationunit] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] 
											INNER JOIN [Billing].[Accounts] AS [AC] ON [AC].[BranchOfficeOrganizationUnitId] = [BOOU].[Id]
											WHERE [AC].[Id] = [LM].[AccountId]),
				[Dg_comment] = [LM].[Comment],
				[Dg_inspectorid] = @InspectorUserId
		FROM [{0}].[dbo].[Dg_limitExtensionBase] AS [CRMLM]
			INNER JOIN [Billing].[Limits] AS [LM] ON [CRMLM].[Dg_limitId] = [LM].[ReplicationCode] AND [LM].[Id] = @Id;		
	END;
	
	RETURN 1;";

        private const string CreateReplicateOperationTypeSPTemplate = @"
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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
	
	RETURN 1;";

        private const string CreateReplicateOrderSPTemplate = @"
    SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;

	SET XACT_ABORT ON;

	DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserDomainName NVARCHAR(250);
    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
    DECLARE @ModifiedByUserDomainName NVARCHAR(250);
	DECLARE @InspectorUserDomainName NVARCHAR(250);
	DECLARE @InspectorUserId UNIQUEIDENTIFIER;
	
	DECLARE @OwnerUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserId UNIQUEIDENTIFIER;
	DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @InspectorUserDomainName = [I].[Account],
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[Orders] AS [TBL]
        INNER JOIN [Billing].[OrderExtensions] AS [TBLEX] ON [TBLEX].[Id] = [TBL].[Id]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
        LEFT OUTER JOIN [Security].[Users] AS [I] ON [I].[Id] = [TBLEX].[InspectorCode]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

	-- get inspector user CRM UserId
    IF (@InspectorUserDomainName IS NOT NULL)
	    SELECT @InspectorUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @InspectorUserDomainName;

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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_orderBase] WHERE [Dg_orderId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN
	
		INSERT INTO [{0}].[dbo].[Dg_orderBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_orderId]
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
           @CreatedByUserId				-- <CreatedBy, uniqueidentifier,>
           ,[Orders].[CreatedOn]		-- <CreatedOn, datetime,>
			,CASE WHEN [Orders].[IsDeleted] = 1 THEN 2 ELSE 0 END -- <DeletionStateCode, int,>
           ,[Orders].[ReplicationCode]	-- <Dg_orderId, uniqueidentifier,>
           ,NULL						-- <ImportSequenceNumber, int,>
		   ,ISNULL(@ModifiedByUserId, @CreatedByUserId)				-- <ModifiedBy, uniqueidentifier,>
		   ,[Orders].[ModifiedOn]		-- <ModifiedOn, datetime,>
           ,NULL						-- <OverriddenCreatedOn, datetime,>
           ,@OwnerUserBusinessUnitId	-- <OwningBusinessUnit, uniqueidentifier,>
			,CASE WHEN [Orders].[IsActive] = 1 THEN 0 ELSE 1 END -- <statecode, int,>
			,CASE WHEN [Orders].[IsActive] = 1 THEN 1 ELSE 2 END -- <statuscode, int,>
           ,NULL						-- <TimeZoneRuleVersionNumber, int,>
           ,NULL						-- <UTCConversionTimeZoneCode, int,>
           ,@OwnerUserId				-- <OwningUser, uniqueidentifier,>)
		FROM [Billing].[Orders] AS [Orders]
		WHERE [Orders].[Id] = @Id;


		INSERT INTO [{0}].[dbo].[Dg_orderExtensionBase]
           ([Dg_amounttowithdraw]
		   ,[Dg_amountwithdrawn]
           ,[Dg_begindistributiondate]
		   ,[Dg_branchofficeorganizationunit]
		   ,[Dg_opportunityid]
		   ,[Dg_dest_organizationunit]
		   ,[Dg_discountpercent]
           ,[Dg_enddistributiondatefact]
           ,[Dg_enddistributiondateplan]
		   ,[Dg_firmid]
		   ,[Dg_inspector]
           ,[Dg_isterminated]
		   ,[Dg_legalperson]
           ,[Dg_number]
           ,[Dg_regionalnumber]
           ,[Dg_orderId]
           ,[Dg_ordertype]
		   ,[Dg_payablefact]
		   ,[Dg_payableplan]
		   ,[Dg_payableprice]
           ,[Dg_source_organizationunit]
           ,[Dg_terminationreason]
           ,[Dg_workflowstep]
		   ,[Dg_discountreason]
           ,[Dg_discountcomment]
		   ,[Dg_documentsdebt]
		   ,[Dg_documentscomment]
		   ,[Dg_account]
		   ,[Dg_signupdate])
		SELECT
            [OrderExtensions].[AmountToWithdraw]	-- <Dg_amounttowithdraw, decimal(23,10),>
		   ,[OrderExtensions].[AmountWithdrawn]		-- <Dg_amountwithdrawn, decimal(23,10),>
           ,[Orders].[BeginDistributionDate]	-- <Dg_begindistributiondate, datetime,>
		   										-- <Dg_branchofficeorganizationunit, uniqueidentifier,>
           ,(SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].[Id] = [Orders].[BranchOfficeOrganizationUnitId])
		   										-- <Dg_opportunityid, uniqueidentifier,>
           ,(SELECT [Deals].[ReplicationCode] FROM [Billing].[Deals] WHERE [Deals].[Id] = [Orders].[DealId])
		   										-- <Dg_dest_organizationunit, uniqueidentifier,>
           ,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[DestOrganizationUnitId])
		   ,[OrderExtensions].[DiscountPercent]	-- <Dg_discountpercent, decimal(23,10),>
           ,[Orders].[EndDistributionDateFact]	-- <Dg_enddistributiondatefact, datetime,>
           ,[Orders].[EndDistributionDatePlan]	-- <Dg_enddistributiondateplan, datetime,>
		   										-- <Dg_firmid, uniqueidentifier,>
           ,(SELECT [Firms].[ReplicationCode] FROM [BusinessDirectory].[Firms] WHERE [Firms].[Id] = [Orders].[FirmId])
		   ,@InspectorUserId					-- <Dg_inspector, uniqueidentifier,>
           ,[Orders].[IsTerminated]				-- <Dg_isterminated, bit,>
		   										-- <Dg_legalperson, uniqueidentifier,>
           ,(SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [Orders].[LegalPersonId])
           ,[Orders].[Number]					-- <Dg_number, NVARCHAR(200),>
           ,[Orders].[RegionalNumber]
           ,[Orders].[ReplicationCode]			-- <Dg_orderId, uniqueidentifier,>
		   ,[OrderExtensions].[OrderType]		-- <Dg_ordertype, int,>
		   ,[OrderExtensions].[PayableFact]		-- <Dg_payablefact, decimal(23,10),>
		   ,[OrderExtensions].[PayablePlan]		-- <Dg_payableplan, decimal(23,10),>
		   ,[OrderExtensions].[PayablePrice]	-- <Dg_payableprice, decimal(23,10),>
												-- <Dg_source_organizationunit, uniqueidentifier,>
           ,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[SourceOrganizationUnitId])
           ,[OrderExtensions].[TerminationReason]	-- <Dg_terminationreason, int,>
           ,[Orders].[WorkflowStepId]				-- <Dg_workflowstep, int,>	 ## mapping of workflowStepId => AttributePicklistValue.Value ##
		   ,[Orders].[DiscountReasonEnum]
           ,[Orders].[DiscountComment]
		   ,[Orders].[HasDocumentsDebt]				-- <Dg_hasdebt, smallint>
		   ,[Orders].[DocumentsComment]
		   ,(SELECT [AC].[ReplicationCode] FROM [Billing].[Accounts] AS [AC] WHERE [AC].[Id] = [Orders].[AccountId])
		   ,[Orders].[SignupDate]
		FROM [Billing].[Orders]
		JOIN [Billing].[OrderExtensions] ON [OrderExtensions].Id = [Orders].Id
		WHERE [Orders].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN

		UPDATE [CRMO]
			SET 
			  [DeletionStateCode] = CASE WHEN [Orders].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [Orders].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode] = CASE WHEN [Orders].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [Orders].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[Dg_orderBase] AS [CRMO]
			INNER JOIN [Billing].[Orders] ON [CRMO].[Dg_orderId] = [Orders].[ReplicationCode] AND [Orders].[Id] = @Id;


		UPDATE [CRMO]
		   SET 
			   [Dg_amounttowithdraw] = [OrderExtensions].[AmountToWithdraw]
			  ,[Dg_amountwithdrawn]  = [OrderExtensions].[AmountWithdrawn]
			  ,[Dg_begindistributiondate] = [Orders].[BeginDistributionDate]
			  ,[Dg_branchofficeorganizationunit] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].[Id] = [Orders].[BranchOfficeOrganizationUnitId])
			  ,[Dg_opportunityid] = (SELECT [Deals].[ReplicationCode] FROM [Billing].[Deals] WHERE [Deals].[Id] = [Orders].[DealId])
			  ,[Dg_dest_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[DestOrganizationUnitId])
			  ,[Dg_discountpercent] = [OrderExtensions].[DiscountPercent]
			  ,[Dg_enddistributiondatefact] = [Orders].[EndDistributionDateFact]
			  ,[Dg_enddistributiondateplan] = [Orders].[EndDistributionDatePlan]
			  ,[Dg_firmid] = (SELECT [Firms].[ReplicationCode] FROM [BusinessDirectory].[Firms] WHERE [Firms].[Id] = [Orders].[FirmId])
			  ,[Dg_inspector] = @InspectorUserId
			  ,[Dg_isterminated] = [Orders].[IsTerminated]
			  ,[Dg_legalperson]  = (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [Orders].[LegalPersonId])
			  ,[Dg_number] = [Orders].[Number]
			  ,[Dg_regionalnumber] = [Orders].[RegionalNumber]
			  ,[Dg_ordertype] = [OrderExtensions].[OrderType]
			  ,[Dg_payablefact]  = [OrderExtensions].[PayableFact]				
			  ,[Dg_payableplan]  = [OrderExtensions].[PayablePlan]
		      ,[Dg_payableprice] = [OrderExtensions].[PayablePrice]	
			  ,[Dg_source_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[SourceOrganizationUnitId])
			  ,[Dg_terminationreason] = [OrderExtensions].[TerminationReason]
			  ,[Dg_workflowstep] = [Orders].[WorkflowStepId]
			  ,[Dg_discountreason] = [Orders].[DiscountReasonEnum]
			  ,[Dg_discountcomment] = [Orders].[DiscountComment]
			  ,[Dg_documentsdebt] = [Orders].[HasDocumentsDebt]
			  ,[Dg_documentscomment] = [Orders].[DocumentsComment]
			  ,[Dg_account] = (SELECT [AC].[ReplicationCode] FROM [Billing].[Accounts] AS [AC] WHERE [AC].[Id] = [Orders].[AccountId])
			  ,[Dg_signupdate] = [Orders].[SignupDate]
		FROM [{0}].[dbo].[Dg_orderExtensionBase] AS [CRMO]
			INNER JOIN [Billing].[Orders] ON [CRMO].[Dg_orderId] = [Orders].[ReplicationCode] AND [Orders].[Id] = @Id
			JOIN [Billing].[OrderExtensions] ON [OrderExtensions].Id = [Orders].Id
	END;

	RETURN 1;";

        private const string CreateReplicateOrderPositionSPTemplate = @"
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_orderpositionBase] WHERE [Dg_orderpositionId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_orderpositionBase]
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
		
		INSERT INTO [{0}].[dbo].[Dg_orderpositionExtensionBase]
           ([Dg_orderpositionId]
           ,[Dg_amount]
		   ,[Dg_discountinpercent]
		   ,[Dg_order]
           ,[Dg_position])
		SELECT
			 [OP].[ReplicationCode]
			,[OP].[Amount]
			,[OP].[DiscountPercent]				
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
		FROM [{0}].[dbo].[Dg_orderpositionBase] AS [CRMOP]
			INNER JOIN [Billing].[OrderPositions] AS [OP] ON [CRMOP].[Dg_orderpositionId] = [OP].[ReplicationCode] AND [OP].[Id] = @Id;
		
		
		UPDATE [CRMOP]
			SET 
				[Dg_amount] = [OP].[Amount]
			   ,[Dg_discountinpercent] = [OP].[DiscountPercent]	
			   ,[Dg_order]    = (SELECT [O].[ReplicationCode] FROM [Billing].[Orders] AS [O] WHERE [O].[Id] = [OP].[OrderId])
			   ,[Dg_position] = (SELECT [P].[ReplicationCode] FROM [Billing].[Positions] AS [P] 
					INNER JOIN [Billing].[PricePositions] [PP] ON [PP].[PositionId] = [P].[Id] AND [PP].[Id] = [OP].[PricePositionId])
		FROM [{0}].[dbo].[Dg_orderpositionExtensionBase] AS [CRMOP]
			INNER JOIN [Billing].[OrderPositions] AS [OP] ON [CRMOP].[Dg_orderpositionId] = [OP].[ReplicationCode] AND [OP].[Id] = @Id;		
	END;
	
	RETURN 1;";

        private const string CreateReplicateOrganizationUnitSPTemplate = @"
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

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[OrganizationUnits] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_organizationunitBase] WHERE [Dg_organizationunitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_organizationunitBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_organizationunitId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OrganizationId]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[OU].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OU].[ReplicationCode],		-- <Dg_organizationunitId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[OU].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_organizationunitExtensionBase]
           ([Dg_name]
           ,[Dg_organizationunitId])
		SELECT
			 [OU].[Name]				-- <Dg_name, NVARCHAR(100),>
			,[OU].[ReplicationCode]		-- <Dg_organizationunitId, uniqueidentifier,>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMOU]
			SET 
			  [DeletionStateCode] = CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OU].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_organizationunitBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
		
		UPDATE [CRMOU]
		SET 
		      [Dg_name] = [OU].[Name]
		FROM [{0}].[dbo].[Dg_organizationunitExtensionBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const string CreateReplicatePositionSPTemplate = @"
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
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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

        private const string CreateReplicateCategorySPTemplate = @"
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

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Categories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_categoryBase] WHERE [Dg_categoryId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_categoryBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_categoryId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OrganizationId]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[C].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [C].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[C].[ReplicationCode],		-- <Dg_categoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[C].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [C].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [C].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[Categories] AS [C]
		WHERE [C].[Id] = @Id;
		
		INSERT INTO [{0}].[dbo].[Dg_categoryExtensionBase]
           ([Dg_categoryId]
           ,[Dg_level]
           ,[Dg_name]
           ,[dg_parentcategory])
		SELECT
			 [C].[ReplicationCode]
			,[C].[Level]				-- <Dg_level, int,>
			,[C].[Name]					-- <Dg_name, int,>
										-- <dg_parentcategory, uniqueidentifier,>
			,(SELECT [PC].[ReplicationCode] FROM [BusinessDirectory].[Categories] AS [PC] WHERE [PC].[Id] = [C].[ParentId])
		FROM [BusinessDirectory].[Categories] AS [C]
		WHERE [C].[Id] = @Id;


		INSERT INTO [{0}].[dbo].[dg_dg_category_dg_organizationunitBase]
           ([dg_dg_category_dg_organizationunitId]
           ,[dg_categoryid]
           ,[dg_organizationunitid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[OU].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [COU].[CategoryId]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [OU].[Id] = [COU].[OrganizationUnitId]
		WHERE [COU].[CategoryId] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMC]
			SET 
			  [DeletionStateCode] = CASE WHEN [C].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [C].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [C].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [C].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_categoryBase] AS [CRMC]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMC].[Dg_categoryId] = [C].[ReplicationCode] AND [C].[Id] = @Id;
		
		
		UPDATE [CRMC]
		   SET
			   [Dg_level] = [C].[Level]
			  ,[Dg_name]  = [C].[Name]
			  ,[dg_parentcategory] = (SELECT [PC].[ReplicationCode] FROM [BusinessDirectory].[Categories] AS [PC] WHERE [PC].[Id] = [C].[ParentId])
		FROM [{0}].[dbo].[Dg_categoryExtensionBase] AS [CRMC]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMC].[Dg_categoryId] = [C].[ReplicationCode] AND [C].[Id] = @Id;
		
		-----------------------------------------------------------------------------------------------------------------------------
		------------------------- [{0}].[dbo].[dg_dg_category_dg_organizationunitBase] ------------------------
		-----------------------------------------------------------------------------------------------------------------------------
		INSERT INTO [{0}].[dbo].[dg_dg_category_dg_organizationunitBase]
           ([dg_dg_category_dg_organizationunitId]
           ,[dg_categoryid]
           ,[dg_organizationunitid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[OU].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [COU].[CategoryId]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [OU].[Id] = [COU].[OrganizationUnitId]
		WHERE [COU].[CategoryId] = @Id
			AND [COU].IsDeleted = 0
			AND NOT EXISTS (
				SELECT 1 
				FROM [{0}].[dbo].[dg_dg_category_dg_organizationunitBase] AS [CRMCOU]
				WHERE [CRMCOU].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMCOU].[dg_organizationunitid] = [OU].[ReplicationCode]
			)

		DELETE [CRMCOU]
		FROM [{0}].[dbo].[dg_dg_category_dg_organizationunitBase] AS [CRMCOU]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [CRMCOU].[dg_categoryid] = [C].[ReplicationCode]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMCOU].[dg_organizationunitid] = [OU].[ReplicationCode]
			INNER JOIN [BusinessDirectory].[CategoryOrganizationUnits] AS [COU]
				ON [COU].[CategoryId] = [C].[Id] AND [COU].[OrganizationUnitId] = [OU].[Id]
		WHERE [COU].[IsDeleted] = 1;
		-----------------------------------------------------------------------------------------------------------------------------
		------------------------- [{0}].[dbo].[dg_dg_category_dg_organizationunitBase] ------------------------
		-----------------------------------------------------------------------------------------------------------------------------
	END;
	
	RETURN 1;";

        private const string CreateReplicateFirmSPTemplate = @"
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
	FROM [BusinessDirectory].[Firms] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_firmBase] WHERE [Dg_firmId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_firmBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_firmId]
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
			[F].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [F].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[F].[ReplicationCode],		-- <Dg_firmId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[F].[ModifiedOn],			-- <ModifiedOn, datetime,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,	-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [F].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [F].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>					
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL,						-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId				-- <OwningUser, uniqueidentifier,>
		FROM [BusinessDirectory].[Firms] AS [F]
		WHERE [F].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_firmExtensionBase]
           ([Dg_account]
		   ,[Dg_closedforascertainment]
           ,[Dg_firmId]
		   ,[Dg_lastqualifytime]
           ,[Dg_lastdisqualifytime]
           ,[Dg_markettype]
           ,[Dg_name]
		   ,[Dg_organizationunit]
           ,[Dg_promisingscore]
		   ,[Dg_territory]
           ,[Dg_usingothermedia]
           ,[Dg_ProductType]
           ,[Dg_BudgetType]
           ,[Dg_Geolocation]
           ,[Dg_InCityBranchesAmount]
           ,[Dg_OutCityBranchesAmount]
           ,[Dg_StaffAmount]
		)
		SELECT
													-- <dg_account, uniqueidentifier,>
			(SELECT [C].[ReplicationCode] FROM [Billing].[Clients] AS [C] WHERE [C].[Id] = [F].[ClientId])
			,[F].[ClosedForAscertainment]			-- <Dg_closedforascertainment, bit,>
			,[F].[ReplicationCode]					-- <Dg_firmId, uniqueidentifier,>
			,[F].[LastQualifyTime]					-- <Dg_lastqualifytime, datetime,>
			,[F].[LastDisqualifyTime]				-- <Dg_lastdisqualifytime, datetime,>
			,[F].[MarketType]						-- <Dg_markettype, int,>
			,[F].[Name]								-- <Dg_name, NVARCHAR(100),>
													-- <Dg_organizationunit, uniqueidentifier,>
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [F].[OrganizationUnitId])
			,[F].[PromisingScore]					-- <Dg_promisingscore, int,>
													-- <dg_territory, uniqueidentifier,>
			,(SELECT [T].[ReplicationCode] FROM [BusinessDirectory].[Territories] AS [T] WHERE [T].[Id] = [F].[TerritoryId])
			,[F].[UsingOtherMedia]					-- <Dg_usingothermedia, int,>
			,[F].[ProductType]						-- <Dg_ProductType, int,>
			,[F].[BudgetType]						-- <Dg_BudgetType, int,>
			,[F].[Geolocation]						-- <Dg_Geolocation, int,>
			,[F].[InCityBranchesAmount]				-- <Dg_InCityBranchesAmount, int,>
			,[F].[OutCityBranchesAmount]			-- <Dg_OutCityBranchesAmount, int,>
			,[F].[StaffAmount]						-- <Dg_StaffAmount, int,>
			
		FROM [BusinessDirectory].[Firms] AS [F]
		WHERE [F].[Id] = @Id;
		
	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMF]
			SET 
			  [DeletionStateCode] = CASE WHEN [F].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [F].[ModifiedOn]
			  --,[OrganizationId] = <OrganizationId, uniqueidentifier,>
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [F].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [F].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[Dg_firmBase] AS [CRMF]
			INNER JOIN [BusinessDirectory].[Firms] AS [F] ON [CRMF].[Dg_firmId] = [F].[ReplicationCode] AND [F].[Id] = @Id;
		
		UPDATE [CRMF]
		SET
			   [Dg_account] = (SELECT [C].[ReplicationCode] FROM [Billing].[Clients] AS [C] WHERE [C].[Id] = [F].[ClientId])
			  ,[Dg_closedforascertainment] = [F].[ClosedForAscertainment]
			  ,[Dg_lastqualifytime] = [F].[LastQualifyTime]
			  ,[Dg_lastdisqualifytime] = [F].[LastDisqualifyTime]
			  ,[Dg_markettype] = [F].[MarketType]
			  ,[Dg_name] = [F].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [F].[OrganizationUnitId])
			  ,[Dg_promisingscore] = [F].[PromisingScore]
			  ,[Dg_territory] = (SELECT [T].[ReplicationCode] FROM [BusinessDirectory].[Territories] AS [T] WHERE [T].[Id] = [F].[TerritoryId])
		      ,[Dg_usingothermedia] = [F].[UsingOtherMedia]
		      ,[Dg_ProductType] = [F].[ProductType]
		      ,[Dg_BudgetType] = [F].[BudgetType]
		      ,[Dg_Geolocation] = [F].[Geolocation]
		      ,[Dg_InCityBranchesAmount] = [F].[InCityBranchesAmount]
		      ,[Dg_OutCityBranchesAmount] = [F].[OutCityBranchesAmount]
		      ,[Dg_StaffAmount] = [F].[StaffAmount]
		FROM [{0}].[dbo].[Dg_firmExtensionBase] AS [CRMF]
			INNER JOIN [BusinessDirectory].[Firms] AS [F] ON [CRMF].[Dg_firmId] = [F].[ReplicationCode]	AND [F].[Id] = @Id;
	END;
	
	RETURN 1;";

        private const string CreateReplicateFirmAddressSPTemplate = @"
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

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
		   @OwnerUserDomainName = [O].[Account], 
		   @CreatedByUserDomainName = [C].[Account], 
		   @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[FirmAddresses] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
		LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
		LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
	WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_firmaddressBase] WHERE [Dg_firmaddressId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_firmaddressBase]
		   ([CreatedBy]
		   ,[CreatedOn]
		   ,[DeletionStateCode]
		   ,[Dg_firmaddressId]
		   ,[ImportSequenceNumber]
		   ,[ModifiedBy]
		   ,[ModifiedOn]
		   ,[OrganizationId]
		   ,[OverriddenCreatedOn]
		   ,[statecode]
		   ,[statuscode]
		   ,[TimeZoneRuleVersionNumber]
		   ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[FA].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[FA].[ReplicationCode],				-- <Dg_firmaddressId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[FA].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[FirmAddresses] AS [FA]
		WHERE [FA].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_firmaddressExtensionBase]
		   ([Dg_address]
		   ,[Dg_firmaddressId]
		   ,[Dg_closedforascertainment]
		   ,[Dg_firm])
		SELECT
			 [FA].[Address]
			,[FA].[ReplicationCode]
			,[FA].[ClosedForAscertainment]
			,(SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [FA].[FirmId])
		FROM [BusinessDirectory].[FirmAddresses] AS [FA]
		WHERE [FA].[Id] = @Id;
		
		INSERT INTO [{0}].[dbo].[dg_dg_category_dg_firmaddressBase]
		   ([dg_dg_category_dg_firmaddressId]
		   ,[dg_categoryid]
		   ,[dg_firmaddressid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[FA].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
		WHERE [CFA].[FirmAddressId] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMFA]
			SET 
			  [DeletionStateCode] = CASE WHEN [FA].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [FA].[ModifiedOn]
			  --,[OrganizationId] = <OrganizationId, uniqueidentifier,>
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [FA].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [FA].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_firmaddressBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode] AND [FA].[Id] = @Id;
		
		
		UPDATE [CRMFA]
		   SET
			   [Dg_address] = [FA].[Address]
			  ,[Dg_closedforascertainment] = [FA].[ClosedForAscertainment]
			  ,[Dg_firm]    = (SELECT [F].[ReplicationCode] FROM [BusinessDirectory].[Firms] AS [F] WHERE [F].[Id] = [FA].[FirmId])
		FROM [{0}].[dbo].[Dg_firmaddressExtensionBase] AS [CRMFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [CRMFA].[Dg_firmaddressId] = [FA].[ReplicationCode] AND [FA].[Id] = @Id;
				
		------------------------------------------------------------------------------------------------------------------------
		---------------------------------------- * dg_dg_category_dg_firmaddressBase * -----------------------------------------
		------------------------------------------------------------------------------------------------------------------------
		INSERT INTO [{0}].[dbo].[dg_dg_category_dg_firmaddressBase]
		   ([dg_dg_category_dg_firmaddressId]
		   ,[dg_categoryid]
		   ,[dg_firmaddressid])
		SELECT
			NEWID(),
			[C].[ReplicationCode],
			[FA].[ReplicationCode]
		FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
		WHERE [CFA].[FirmAddressId] = @Id
			AND [CFA].[IsDeleted] = 0
			AND NOT EXISTS (
				SELECT 1
				FROM [{0}].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA]
				WHERE [CRMFA].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA].[dg_firmaddressid] = [FA].[ReplicationCode]
			);

		DELETE [CRMFA]
		FROM [BusinessDirectory].[CategoryFirmAddresses] AS [CFA]
			INNER JOIN [BusinessDirectory].[FirmAddresses] AS [FA] ON [FA].[Id] = [CFA].[FirmAddressId]
			INNER JOIN [BusinessDirectory].[Categories] AS [C] ON [C].[Id] = [CFA].[CategoryId]
			INNER JOIN [{0}].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA]
				ON	[CRMFA].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA].[dg_firmaddressid] = [FA].[ReplicationCode]
		WHERE [CFA].[FirmAddressId] = @Id
			AND [CFA].[IsDeleted] = 1
			AND EXISTS (
				SELECT 1
				FROM [{0}].[dbo].[dg_dg_category_dg_firmaddressBase] AS [CRMFA2]
				WHERE [CRMFA2].[dg_categoryid] = [C].[ReplicationCode]
					AND [CRMFA2].[dg_firmaddressid] = [FA].[ReplicationCode]
			);
		------------------------------------------------------------------------------------------------------------------------
		---------------------------------------- * dg_dg_category_dg_firmaddressBase * -----------------------------------------
		------------------------------------------------------------------------------------------------------------------------
		
	END;
	
	RETURN 1;";

        private const string CreateReplicateTerritorySPTemplate = @"
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

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Territories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;
    
	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_territoryBase] WHERE [Dg_territoryId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_territoryBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_territoryId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OrganizationId]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[T].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [T].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[T].[ReplicationCode],		-- <Dg_territoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[T].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_territoryExtensionBase]
           ([Dg_name]
           ,[Dg_territoryId]
           ,[dg_organizationunit])
		SELECT
			 [T].[Name]
			,[T].[ReplicationCode]
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMT]
			SET 
			  [DeletionStateCode] = CASE WHEN [T].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [T].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_territoryBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
		
		
		UPDATE [CRMT]
		   SET 
		       [Dg_name] = [T].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [{0}].[dbo].[Dg_territoryExtensionBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
	END;
	
	RETURN 1;";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateAccount, CreateReplicateAccountSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateAccountDetail, CreateReplicateAccountDetailSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateBargain, CreateReplicateBargainSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateBranchOffice, CreateReplicateBranchOfficeSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateBranchOfficeOrganizationUnit, CreateReplicateBranchOfficeOrganizationUnitSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateClient, CreateReplicateClientSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateContact, CreateReplicateContactSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateCurrency, CreateReplicateCurrencySPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateDeal, CreateReplicateDealSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateLegalPerson, CreateReplicateLegalPersonSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateLimit, CreateReplicateLimitSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOperationType, CreateReplicateOperationTypeSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOrder, CreateReplicateOrderSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOrderPosition, CreateReplicateOrderPositionSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOrganizationUnit, CreateReplicateOrganizationUnitSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicatePosition, CreateReplicatePositionSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateCategory, CreateReplicateCategorySPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateFirm, CreateReplicateFirmSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateFirmAddress, CreateReplicateFirmAddressSPTemplate, context.CrmDatabaseName);
            AlterOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateTerritory, CreateReplicateTerritorySPTemplate, context.CrmDatabaseName);
        }

        private static void AlterOrCreateReplicationSP(IMigrationContext context, SchemaQualifiedObjectName storedProcQualifiedName, string textBodyTemplate, string environmentNumberString)
        {
            var spTextBody = string.Format(textBodyTemplate, environmentNumberString);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, storedProcQualifiedName, spTextBody);
        }
    }
}