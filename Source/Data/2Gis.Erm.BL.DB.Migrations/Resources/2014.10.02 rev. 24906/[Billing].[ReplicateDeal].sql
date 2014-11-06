/****** Object:  StoredProcedure [Billing].[ReplicateDeal]    Script Date: 01.09.2014 10:28:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   23.07.2014, i.maslennikov: drop deal profit indicators 
--   12.08.2014, y.baranikhin: добавилась репликация полей по рекламной кампании
--   11.09.2014, a.tukaev: выпиливаем like при поиске пользователя по account
--   02.10.2014, a.rechkalov: выполнил слияние двух предыдущих изменений (были сделаны в разных ветках)
ALTER PROCEDURE [Billing].[ReplicateDeal]
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

	DECLARE @IncreaseSalesGoal int = 1;
	DECLARE @AttractAudienceToSiteGoal int = 2;
	DECLARE @IncreasePhoneCallsGoal int = 4;
	DECLARE @IncreaseBrandAwarenessGoal int = 8;

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
		WHEN ([D].[IsActive] = 0) THEN 1
		END 
	FROM [Billing].[Deals] AS [D]
	WHERE [D].Id = @Id;

	SET @StatusCode = CASE
		WHEN (@StateCode = 0) THEN 1
		WHEN (@StateCode = 1) THEN 3
	END  

	--------------------------------------------------------------------------
	-- INSERT NEW
	--------------------------------------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[OpportunityBase] WHERE [OpportunityId] = @CrmId)
	BEGIN

		-- OpportunityBase
		INSERT INTO [DoubleGis_MSCRM].[dbo].[OpportunityBase]
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
		INSERT INTO [DoubleGis_MSCRM].[dbo].[OpportunityExtensionBase]
			([OpportunityId]
			,[dg_closereason]
			,[dg_closereasonother]
			,[dg_dealstage]
			,[dg_firm]
			,[dg_isactive]
			,[dg_startreason]
			,[dg_currency]
			,[Dg_advertisingCampaignBeginDate]
			,[Dg_advertisingCampaignEndDate]
			,[Dg_agencyFee]
			,[Dg_advertisingCampaignGoalsText]
			,[Dg_paymentFormat]
			,[Dg_increaseSalesGoal]
			,[Dg_attractAudienceToSiteGoal]
			,[Dg_increasePhoneCallsGoal]
			,[Dg_increaseBrandAwarenessGoal]
			,[Dg_bargain])
		SELECT
			[D].ReplicationCode
			,[D].CloseReason
			,[D].CloseReasonOther
			,[D].DealStage
			,(SELECT [FR].ReplicationCode FROM [BusinessDirectory].[Firms] AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[D].IsActive
			,[D].StartReason
			,(SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
			,[D].AdvertisingCampaignBeginDate
			,[D].AdvertisingCampaignEndDate
			,[D].AgencyFee
			,[D].AdvertisingCampaignGoalText
			,[D].PaymentFormat
			,[D].AdvertisingCampaignGoals & @IncreaseSalesGoal
			,[D].AdvertisingCampaignGoals & @AttractAudienceToSiteGoal
			,[D].AdvertisingCampaignGoals & @IncreasePhoneCallsGoal
			,[D].AdvertisingCampaignGoals & @IncreaseBrandAwarenessGoal
			,(SELECT [B].ReplicationCode FROM [Billing].[Bargains] AS [B] WHERE [B].Id = [D].BargainId)
		FROM [Billing].[Deals] AS [D]
		WHERE [D].Id = @Id;
	END

	-------------------------------------------------------
	-- UPDATE EXISTING
	-------------------------------------------------------
	ELSE
	BEGIN
		
		-- OpportunityBase
		UPDATE [DoubleGis_MSCRM].[dbo].[OpportunityBase]
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
		FROM [DoubleGis_MSCRM].[dbo].[OpportunityBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id;
		
		-- OpportunityExtensionBase
		UPDATE [DoubleGis_MSCRM].[dbo].[OpportunityExtensionBase]
		   SET 
			[OpportunityId] = [D].ReplicationCode
			,[dg_closereason]  = [D].CloseReason
			,[dg_closereasonother] = [D].CloseReasonOther
			,[dg_dealstage] = [D].DealStage
			,[dg_firm]    = (SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[dg_isactive] = [D].IsActive
			,[dg_startreason] = [D].StartReason
			,[dg_currency] = (SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
			,[Dg_advertisingCampaignBeginDate] = [D].AdvertisingCampaignBeginDate
			,[Dg_advertisingCampaignEndDate] = [D].AdvertisingCampaignEndDate
			,[Dg_agencyFee] = [D].AgencyFee
			,[Dg_advertisingCampaignGoalsText] = [D].AdvertisingCampaignGoalText
			,[Dg_paymentFormat] = [D].PaymentFormat
			,[Dg_increaseSalesGoal] = [D].AdvertisingCampaignGoals & @IncreaseSalesGoal
			,[Dg_attractAudienceToSiteGoal] = [D].AdvertisingCampaignGoals & @AttractAudienceToSiteGoal
			,[Dg_increasePhoneCallsGoal] = [D].AdvertisingCampaignGoals & @IncreasePhoneCallsGoal
			,[Dg_increaseBrandAwarenessGoal] = [D].AdvertisingCampaignGoals & @IncreaseBrandAwarenessGoal
			,[Dg_bargain] = (SELECT [B].ReplicationCode FROM [Billing].[Bargains] AS [B] WHERE [B].Id = [D].BargainId)
		FROM [{0}].[dbo].[OpportunityExtensionBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id
	END;

	--------------------------------------------------------------------------
	-- CLOSED DEAL
	--------------------------------------------------------------------------

	-- trying to replicate closed deal if CloseDate is set
	IF EXISTS (SELECT 1 FROM [Billing].[Deals] WHERE [CloseDate] IS NOT NULL AND [Id] = @Id)
	BEGIN
		
	-- add new closed deal
	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase]
					WHERE [RegardingObjectId] = @CrmId
						  AND [RegardingObjectTypeCode] = 3 -- 3 is opportunity object type code
						  AND [ActualEnd] = (SELECT [CloseDate] FROM [Billing].[Deals] WHERE [Id] = @Id))
	BEGIN
		SET @ActivityId = NEWID()

		-- ActivityPointerBase
		INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPointerBase]
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
		INSERT INTO [DoubleGis_MSCRM].[dbo].[OpportunityCloseBase]
				([ActivityId]
				,[TransactionCurrencyId]
				,[ExchangeRate]
				)
			SELECT
				@ActivityId
				-- TODO: репликация требует указания TransactionCurrency, она никак не связана с нашей dg_currency, разобраться
				,(SELECT TOP 1 [TransactionCurrencyId] FROM [DoubleGis_MSCRM].[dbo].[TransactionCurrencyBase])
				,1 -- echange rate is always 1
		FROM [Billing].[Deals] AS [D]
		WHERE [D].[Id] = @Id;

		-- ActivityPartyBase - opprtunityclose-user relation
		INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
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
		WHERE [D].[Id] = @Id;

		-- ActivityPartyBase - opprtunityclose-opprtunity relation
		INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
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
		WHERE [D].[Id] = @Id;

	END

	ELSE 

	-- update existing closed deal, very rare case, not tested, may contain bugs
	BEGIN
		-- ActivityPointerBase
		UPDATE [DoubleGis_MSCRM].[dbo].[ActivityPointerBase]
		   SET [RegardingObjectIdName] = [D].[Name]
		FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] AS [AP]
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [RegardingObjectId] AND [RegardingObjectTypeCode] = 3 AND [D].[Id] = @Id

		-- OpportunityCloseBase
		-- no updates needed

		-- ActivityPartyBase - opprtunityclose-user
		-- no updates needed

		-- ActivityPartyBase - opprtunityclose-opprtunity relation
		UPDATE [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
		   SET [PartyIdName] = [D].[Name]
		FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] AS [APB]
			INNER JOIN [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] as [AP] ON [AP].[ActivityId] = [APB].[PartyId] AND [APB].[PartyObjectTypeCode] = 3 AND [AP].[RegardingObjectTypeCode] = 3
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [AP].[RegardingObjectId] AND [D].[Id] = @Id

	END
	END

	RETURN 1;


