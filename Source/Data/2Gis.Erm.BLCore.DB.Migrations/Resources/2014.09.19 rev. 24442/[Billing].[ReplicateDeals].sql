CREATE PROCEDURE [Billing].[ReplicateDeals]
    @Ids [Shared].[Int64IdsTableType] READONLY
AS
    SET NOCOUNT ON;

    IF NOT EXISTS ( SELECT  1
                    FROM    @Ids )
        BEGIN
            RETURN 0;
        END;

    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRAN;

        MERGE [DoubleGis_MSCRM].[dbo].[OpportunityBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [OpportunityId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 3 END AS [statuscode],
                        [C].[ReplicationCode] AS [AccountId],
                        [TBL].[CloseDate] AS [ActualCloseDate],
                        [TBL].[Name] AS [Name]
              FROM      [Billing].[Deals] AS [TBL]
                        JOIN [Billing].[Clients] AS [C] ON [C].[Id] = [TBL].[ClientId]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[OpportunityId] = [New].[OpportunityId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Current].[DeletionStateCode] = [New].[DeletionStateCode],
                    [Current].[ModifiedBy] = [New].[ModifiedBy],
                    [Current].[ModifiedOn] = [New].[ModifiedOn],
                    [Current].[statecode] = [New].[statecode],
                    [Current].[statuscode] = [New].[statuscode],
                    [Current].[OwningUser] = [New].[OwningUser],
                    [Current].[OwningBusinessUnit] = [New].[OwningBusinessUnit],
                    [Current].[AccountId] = [New].[AccountId],
                    [Current].[ActualCloseDate] = [New].[ActualCloseDate],
                    [Current].[Name] = [New].[Name]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [OpportunityId],
                     [CreatedOn],
                     [DeletionStateCode],
                     [CreatedBy],
                     [ModifiedBy],
                     [ModifiedOn],
                     [OwningBusinessUnit],
                     [statecode],
                     [statuscode],
                     [OwningUser],
                     [AccountId],
                     [ActualCloseDate],
                     [Name] )
            VALUES ( [New].[OpportunityId],
                     [New].[CreatedOn],
                     [New].[DeletionStateCode],
                     [New].[CreatedBy],
                     [New].[ModifiedBy],
                     [New].[ModifiedOn],
                     [New].[OwningBusinessUnit],
                     [New].[statecode],
                     [New].[statuscode],
                     [New].[OwningUser],
                     [New].[AccountId],
                     [New].[ActualCloseDate],
                     [New].[Name] );
        
        MERGE [DoubleGis_MSCRM].[dbo].[OpportunityExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [OpportunityId],
                        [TBL].[CloseReason] AS [dg_closereason],
                        [TBL].[CloseReasonOther] AS [dg_closereasonother],
                        [TBL].[DealStage] AS [dg_dealstage],
                        [F].[ReplicationCode] AS [dg_firm],
                        [TBL].[IsActive] AS [dg_isactive],
                        [TBL].[StartReason] AS [dg_startreason],
                        [C].[ReplicationCode] AS [dg_currency]
              FROM      [Billing].[Deals] AS [TBL]
                        JOIN [BusinessDirectory].[Firms] [F] ON [F].[Id] = [TBL].[MainFirmId]
                        JOIN [Billing].[Currencies] [C] ON [C].[Id] = [TBL].[CurrencyId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[OpportunityId] = [New].[OpportunityId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Current].[OpportunityId] = [New].[OpportunityId],
                    [Current].[dg_closereason] = [New].[dg_closereason],
                    [Current].[dg_closereasonother] = [New].[dg_closereasonother],
                    [Current].[dg_dealstage] = [New].[dg_dealstage],
                    [Current].[dg_firm] = [New].[dg_firm],
                    [Current].[dg_isactive] = [New].[dg_isactive],
                    [Current].[dg_startreason] = [New].[dg_startreason],
                    [Current].[dg_currency] = [New].[dg_currency]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [OpportunityId],
                     [dg_closereason],
                     [dg_closereasonother],
                     [dg_dealstage],
                     [dg_firm],
                     [dg_isactive],
                     [dg_startreason],
                     [dg_currency] )
            VALUES ( [New].[OpportunityId],
                     [New].[dg_closereason],
                     [New].[dg_closereasonother],
                     [New].[dg_dealstage],
                     [New].[dg_firm],
                     [New].[dg_isactive],
                     [New].[dg_startreason],
                     [New].[dg_currency] );



	   -- closed deal handling
        DECLARE @ClosedDealIds AS [Shared].[Int64IdsTableType];

	   -- add new closed deal
        INSERT  INTO @ClosedDealIds
                ( Id )
                SELECT  [Id]
                FROM    [Billing].[Deals]
                WHERE   [Id] IN ( SELECT    [Id]
                                  FROM      @Ids )
                        AND [CloseDate] IS NOT NULL;

        DECLARE @OpportunityActivityInfo TABLE
            (
              [ActivityId] UNIQUEIDENTIFIER NOT NULL
                                            PRIMARY KEY,
              [OpportunityId] UNIQUEIDENTIFIER NOT NULL,
              [OwningUser] UNIQUEIDENTIFIER
            );

        INSERT  INTO [DoubleGis_MSCRM].[dbo].[ActivityPointerBase]
                ( [ActivityId],
                  [ActualEnd],
                  [Description],
                  [OwningBusinessUnit],
                  [IsBilled],
                  [RegardingObjectIdName],
                  [StateCode],
                  [ModifiedOn],
                  [StatusCode],
                  [Subject],
                  [IsWorkflowCreated],
                  [CreatedBy],
                  [OwningUser],
                  [ModifiedBy],
                  [RegardingObjectIdDsc],
                  [RegardingObjectId],
                  [RegardingObjectTypeCode],
                  [DeletionStateCode],
                  [CreatedOn],
                  [TimeZoneRuleVersionNumber],
                  [ActivityTypeCode] )
        OUTPUT  [Inserted].[ActivityId],
                [Inserted].[RegardingObjectId],
                [Inserted].[OwningUser]
                INTO @OpportunityActivityInfo ( [ActivityId], [OpportunityId], [OwningUser] )
                SELECT  NEWID(),
                        [D].[CloseDate],
                        [D].[Comment],
                        [OCU].[BusinessUnitId],
                        0,
                        [D].[Name],
                        CASE WHEN [D].[IsActive] = 1 THEN 0 ELSE 1 END,
                        [D].[ModifiedOn],
                        CASE WHEN [D].[IsActive] = 1 THEN 1 ELSE 3 END,
                        [D].[Name],
                        0,
                        [CCU].[SystemUserId],
                        [OCU].[SystemUserId],
                        [MCU].[SystemUserId],
                        0,
                        [D].[ReplicationCode],
                        3, -- 3 is opportunity object type code
                        CASE WHEN [D].[IsDeleted] = 1 THEN 2 ELSE 0 END,
                        [D].[CreatedOn],
                        0,
                        4208 -- 4208 is opportunityclose activity type code
                FROM    [Billing].[Deals] AS [D]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [D].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [D].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [D].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
                WHERE   [D].[Id] IN ( SELECT    [Id]
                                      FROM      @ClosedDealIds );


	   	-- OpportunityCloseBase
		-- TODO: репликация требует указания TransactionCurrency, она никак не связана с нашей dg_currency, разобраться
        DECLARE @TransactionCurrency UNIQUEIDENTIFIER;
        SET @TransactionCurrency = ( SELECT TOP 1
                                            [TransactionCurrencyId]
                                     FROM   [DoubleGis_MSCRM].[dbo].[TransactionCurrencyBase]
                                   );
		
        INSERT  INTO [DoubleGis_MSCRM].[dbo].[OpportunityCloseBase]
                ( [ActivityId],
                  [TransactionCurrencyId],
                  [ExchangeRate] )
                SELECT  [ActivityId],
                        @TransactionCurrency,
                        1 -- echange rate is always 1
                FROM    @OpportunityActivityInfo;


	   -- ActivityPartyBase - opprtunityclose-user relation
        INSERT  INTO [DoubleGis_MSCRM].[dbo].ActivityPartyBase
                ( [ActivityPartyId],
                  [ActivityId],
                  [PartyId],
                  [PartyObjectTypeCode],
                  [ParticipationTypeMask] )
                SELECT  NEWID(),
                        [ActivityId],
                        [OwningUser],
                        8, -- 8 is user object type code
                        9 -- compatibility with dynamics crm 3.0
                FROM    @OpportunityActivityInfo;

    -- ActivityPartyBase - opprtunityclose-opprtunity relation
        INSERT  INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
                ( [ActivityPartyId],
                  [ActivityId],
                  [PartyId],
                  [PartyObjectTypeCode],
                  [ParticipationTypeMask] )
                SELECT  NEWID(),
                        [ActivityId],
                        [OpportunityId],
                        3, -- 3 is opprtunity object type code
                        8 -- compatibility with dynamics crm 3.0
                FROM    @OpportunityActivityInfo;


        COMMIT TRAN;

        RETURN 1;
    END TRY
    BEGIN CATCH

        IF ( XACT_STATE() != 0 )
            BEGIN
                ROLLBACK TRAN;
            END;

        DECLARE @ErrorMessage NVARCHAR(MAX),
            @ErrorSeverity INT,
            @ErrorState INT;

        SELECT  @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        RAISERROR ( @ErrorMessage , @ErrorSeverity , @ErrorState );
    END CATCH;

GO