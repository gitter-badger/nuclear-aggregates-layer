CREATE PROCEDURE [Billing].[ReplicateBargains]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_bargainBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_bargainId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[Bargains] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_bargainId] = [New].[Dg_bargainId] )
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
                     [Dg_bargainId],
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
                     [New].[Dg_bargainId],
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_bargainExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_bargainId],
                        [TBL].[Number] AS [Dg_number],
                        [TBL].[Comment] AS [Dg_comment],
                        [TBL].[SignedOn] AS [Dg_signedon],
                        [LP].[ReplicationCode] AS [Dg_legalperson],
                        [BO].[ReplicationCode] AS [Dg_branchoffice],
                        [TBL].[ClosedOn] AS [Dg_closedon],
                        [TBL].[HasDocumentsDebt] AS [Dg_documentsdebt],
                        [TBL].[DocumentsComment] AS [Dg_documentscomment],
                        [TBL].[BargainKind] AS [Dg_bargainkind],
                        [TBL].[BargainEndDate] AS [Dg_bargainenddate]    
              FROM      [Billing].[Bargains] AS [TBL]
                        JOIN [Billing].[LegalPersons] AS [LP] ON [LP].[Id] = [TBL].[CustomerLegalPersonId]
                        JOIN [Billing].[BranchOffices] AS [BO] ON [BO].[Id] = [TBL].[ExecutorBranchOfficeId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_bargainId] = [New].[Dg_bargainId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Dg_number] = [New].[Dg_number],
                    [Dg_comment] = [New].[Dg_comment],
                    [Dg_signedon] = [New].[Dg_signedon],
                    [Dg_legalperson] = [New].[Dg_legalperson],
                    [Dg_branchoffice] = [New].[Dg_branchoffice],
                    [Dg_closedon] = [New].[Dg_closedon],
                    [Dg_documentsdebt] = [New].[Dg_documentsdebt],
                    [Dg_documentscomment] = [New].[Dg_documentscomment],
                    [Dg_bargainkind] =	[New].[Dg_bargainkind],
                    [Dg_bargainenddate] = [New].[Dg_bargainenddate]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [Dg_number],
                     [Dg_comment],
                     [Dg_signedon],
                     [Dg_legalperson],
                     [Dg_branchoffice],
                     [Dg_bargainId],
                     [Dg_closedon],
                     [Dg_documentsdebt],
                     [Dg_documentscomment],
                     [Dg_bargainkind],
                     [Dg_bargainenddate])
            VALUES ( [New].[Dg_number],
                     [New].[Dg_comment],
                     [New].[Dg_signedon],
                     [New].[Dg_legalperson],
                     [New].[Dg_branchoffice],
                     [New].[Dg_bargainId],
                     [New].[Dg_closedon],
			         [New].[Dg_documentsdebt],
		             [New].[Dg_documentscomment],
			         [New].[Dg_bargainkind],
			         [New].[Dg_bargainenddate] );

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