CREATE PROCEDURE [Billing].[ReplicateOrderPositions]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_orderpositionBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_orderpositionId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[OrderPositions] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_orderpositionId] = [New].[Dg_orderpositionId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Current].[DeletionStateCode] = [New].[DeletionStateCode],
                    [Current].[ModifiedBy] = [New].[ModifiedBy],
                    [Current].[ModifiedOn] = [New].[ModifiedOn],
                    [Current].[OwningBusinessUnit] = [New].[OwningBusinessUnit],
                    [Current].[statecode] = [New].[statecode],
                    [Current].[statuscode] = [New].[statuscode],
                    [Current].[OwningUser] = [New].[OwningUser]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [CreatedBy],
                     [CreatedOn],
                     [DeletionStateCode],
                     [Dg_orderpositionId],
                     [ImportSequenceNumber],
                     [ModifiedBy],
                     [ModifiedOn],
                     [OverriddenCreatedOn],
                     [OwningBusinessUnit],
                     [statecode],
                     [statuscode],
                     [TimeZoneRuleVersionNumber],
                     [UTCConversionTimeZoneCode],
                     [OwningUser] )
            VALUES ( [New].[CreatedBy],
                     [New].[CreatedOn],
                     [New].[DeletionStateCode],
                     [New].[Dg_orderpositionId],
                     NULL,
                     [New].[ModifiedBy],
                     [New].[ModifiedOn],
                     NULL,
                     [New].[OwningBusinessUnit],
                     [New].[statecode],
                     [New].[statuscode],
                     NULL,
                     NULL,
                     [New].[OwningUser] );

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_orderpositionExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_orderpositionId],
                        [TBL].[Amount] AS [Dg_amount],
						[TBL].[DiscountSum] AS [Dg_discountvalue],
                        [TBL].[DiscountPercent] AS [Dg_discountinpercent],
                        [TBL].[CategoryRate] AS [Dg_CategoryRate],
                        [O].[ReplicationCode] AS [Dg_order],
                        [P].[ReplicationCode] AS [Dg_position]
              FROM      [Billing].[OrderPositions] AS [TBL]
                        JOIN [Billing].[Orders] AS [O] ON [O].[Id] = [TBL].[OrderId]
                        JOIN [Billing].[PricePositions] [PP] ON [PP].[Id] = [TBL].[PricePositionId]
						JOIN [Billing].[Positions] [P] ON [P].[Id] = [PP].[PositionId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_orderpositionId] = [New].[Dg_orderpositionId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Dg_amount] = [New].[Dg_amount],
					[Dg_discountvalue] = [New].[Dg_discountvalue],
                    [Dg_discountinpercent] = [New].[Dg_discountinpercent],
                    [Dg_CategoryRate] = [New].[Dg_CategoryRate],
                    [Dg_order] = [New].[Dg_order],
                    [Dg_position] = [New].[Dg_position]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [Dg_orderpositionId],
                     [Dg_amount],
					 [Dg_discountvalue],
                     [Dg_discountinpercent],
                     [Dg_CategoryRate],
                     [Dg_order],
                     [Dg_position] )
            VALUES ( [New].[Dg_orderpositionId],
                     [New].[Dg_amount],
					 [New].[Dg_discountvalue],
                     [New].[Dg_discountinpercent],
                     [New].[Dg_CategoryRate],
                     [New].[Dg_order],
                     [New].[Dg_position] );

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