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
	FROM [DoubleGis{0}_MSCRM].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [DoubleGis{0}_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [DoubleGis{0}_MSCRM].[dbo].[SystemUserBase]
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
		WHEN ([D].[IsActive] = 0 AND [D].[ActualProfit] > 0) THEN 1
		WHEN ([D].[IsActive] = 0 AND [D].[ActualProfit] = 0) THEN 2
		END 
	FROM [Billing].[Deals] AS [D]

	SET @StatusCode = CASE
		WHEN (@StateCode = 0) THEN 1
		WHEN (@StateCode = 1) THEN 3
		WHEN (@StateCode = 2) THEN 5
	END  

	--------------------------------------------------------------------------
	-- INSERT NEW
	--------------------------------------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM [DoubleGis{0}_MSCRM].[dbo].[OpportunityBase] WHERE [OpportunityId] = @CrmId)
	BEGIN

		-- OpportunityBase
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[OpportunityBase]
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
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[OpportunityExtensionBase]
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
			,[D].ActualProfit
			,[D].CloseReason
			,[D].CloseReasonOther
			,[D].DealStage
			,[D].EstimatedProfit
			,(SELECT [FR].ReplicationCode FROM [BusinessDirectory].[Firms] AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[D].IsActive
			,[D].StartReason
			,(SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
		FROM [Billing].[Deals] AS [D]
	END

	-------------------------------------------------------
	-- UPDATE EXISTING
	-------------------------------------------------------
	ELSE
	BEGIN
		
		-- OpportunityBase
		UPDATE [DoubleGis{0}_MSCRM].[dbo].[OpportunityBase]
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
		FROM [DoubleGis{0}_MSCRM].[dbo].[OpportunityBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id;
		
		-- OpportunityExtensionBase
		UPDATE [DoubleGis{0}_MSCRM].[dbo].[OpportunityExtensionBase]
		   SET 
			[OpportunityId] = [D].ReplicationCode
			,[dg_actualprofit] = [D].ActualProfit
			,[dg_closereason]  = [D].CloseReason
			,[dg_closereasonother] = [D].CloseReasonOther
			,[dg_dealstage] = [D].DealStage
			,[dg_estimatedprofit] = [D].EstimatedProfit
			,[dg_firm]    = (SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [D].MainFirmId)
			,[dg_isactive] = [D].IsActive
			,[dg_startreason] = [D].StartReason
			,[dg_currency] = (SELECT [C].ReplicationCode FROM [Billing].[Currencies] AS [C] WHERE [C].Id = [D].CurrencyId)
		FROM [DoubleGis{0}_MSCRM].[dbo].[OpportunityExtensionBase] AS [CRMD]
			INNER JOIN [Billing].[Deals] AS [D] ON [CRMD].[OpportunityId] = [D].[ReplicationCode] AND [D].[Id] = @Id
	END;

	--------------------------------------------------------------------------
	-- CLOSED DEAL
	--------------------------------------------------------------------------

	-- trying to replicate closed deal if CloseDate is set
	IF EXISTS (SELECT 1 FROM [Billing].[Deals] WHERE [CloseDate] IS NOT NULL AND [Id] = @Id)
	BEGIN
		
	-- add new closed deal
	IF NOT EXISTS (SELECT 1 FROM [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase]
					WHERE [RegardingObjectId] = @CrmId
						  AND [RegardingObjectTypeCode] = 3 -- 3 is opportunity object type code
						  AND [ActualEnd] = (SELECT [CloseDate] FROM [Billing].[Deals] WHERE [Id] = @Id))
	BEGIN
		SET @ActivityId = NEWID()

		-- ActivityPointerBase
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase]
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
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[OpportunityCloseBase]
				([ActivityId]
				,[TransactionCurrencyId]
				,[ExchangeRate]
				,[ActualRevenue_Base]
				,[ActualRevenue]
				)
			SELECT
				@ActivityId
				-- TODO: репликация требует указания TransactionCurrency, она никак не связана с нашей dg_currency, разобраться
				,(SELECT TOP 1 [TransactionCurrencyId] FROM [DoubleGis{0}_MSCRM].[dbo].[TransactionCurrencyBase])
				,1 -- echange rate is always 1
				,0
				,[D].ActualProfit
		FROM [Billing].[Deals] AS [D]
		WHERE [D].[Id] = @Id;

		-- ActivityPartyBase - opprtunityclose-user relation
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[ActivityPartyBase]
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
		INSERT INTO [DoubleGis{0}_MSCRM].[dbo].[ActivityPartyBase]
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
		UPDATE [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase]
		   SET [RegardingObjectIdName] = [D].[Name]
		FROM [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase] AS [AP]
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [RegardingObjectId] AND [RegardingObjectTypeCode] = 3 AND [D].[Id] = @Id

		-- OpportunityCloseBase
		UPDATE [DoubleGis{0}_MSCRM].[dbo].[OpportunityCloseBase]
		   SET [ActualRevenue] = [D].[ActualProfit]
		FROM [DoubleGis{0}_MSCRM].[dbo].[OpportunityCloseBase] AS [OC]
			INNER JOIN [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase] as [AP] ON [AP].[ActivityId] = [OC].[ActivityId] AND [AP].[RegardingObjectTypeCode] = 3
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [AP].[RegardingObjectId] AND [D].[Id] = @Id

		-- ActivityPartyBase - opprtunityclose-user
		-- no updates needed

		-- ActivityPartyBase - opprtunityclose-opprtunity relation
		UPDATE [DoubleGis{0}_MSCRM].[dbo].[ActivityPartyBase]
		   SET [PartyIdName] = [D].[Name]
		FROM [DoubleGis{0}_MSCRM].[dbo].[ActivityPartyBase] AS [APB]
			INNER JOIN [DoubleGis{0}_MSCRM].[dbo].[ActivityPointerBase] as [AP] ON [AP].[ActivityId] = [APB].[PartyId] AND [APB].[PartyObjectTypeCode] = 3 AND [AP].[RegardingObjectTypeCode] = 3
			INNER JOIN [Billing].[Deals] AS [D] ON [D].[ReplicationCode] = [AP].[RegardingObjectId] AND [D].[Id] = @Id

	END
	END

	RETURN 1;