CREATE PROCEDURE [Billing].[ReplicateAccountDetails]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_accountdetailBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_accountdetailId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[AccountDetails] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_accountdetailId] = [New].[Dg_accountdetailId] )
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
                     [Dg_accountdetailId],
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
                     [New].[Dg_accountdetailId],
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_accountdetailExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_accountdetailId],
                        [TBL].[Description] AS [Dg_description],
                        [TBL].[Amount] AS [Dg_amount],
                        [TBL].[Comment] AS [Dg_comment],
                        [TBL].[TransactionDate] AS [Dg_transactiondate],
                        [A].[ReplicationCode] AS [Dg_account],
                        [OT].[ReplicationCode] AS [Dg_operationtype]
              FROM      [Billing].[AccountDetails] AS [TBL]
                        JOIN [Billing].[Accounts] AS [A] ON [A].[Id] = [TBL].[AccountId]
                        JOIN [Billing].[OperationTypes] AS [OT] ON [OT].[Id] = [TBL].[OperationTypeId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_accountdetailId] = [New].[Dg_accountdetailId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Current].[Dg_description] = [New].[Dg_description],
                    [Current].[Dg_amount] = [New].[Dg_amount],
                    [Current].[Dg_comment] = [New].[Dg_comment],
                    [Current].[Dg_transactiondate] = [New].[Dg_transactiondate],
                    [Current].[Dg_account] = [New].[Dg_account],
                    [Current].[Dg_operationtype] = [New].[Dg_operationtype]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [Dg_accountdetailId],
                     [Dg_description],
                     [Dg_amount],
                     [Dg_comment],
                     [Dg_transactiondate],
                     [Dg_account],
                     [Dg_operationtype] )
            VALUES ( [New].[Dg_accountdetailId],
                     [New].[Dg_description],
                     [New].[Dg_amount],
                     [New].[Dg_comment],
                     [New].[Dg_transactiondate],
                     [New].[Dg_account],
                     [New].[Dg_operationtype] );

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