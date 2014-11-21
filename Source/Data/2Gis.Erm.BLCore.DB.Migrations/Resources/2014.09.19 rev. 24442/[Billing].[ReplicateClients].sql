CREATE PROCEDURE [Billing].[ReplicateClients]
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

        MERGE [DoubleGis_MSCRM].[dbo].[AccountBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2
                             ELSE 0
                        END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [AccountId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0
                             ELSE 1
                        END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1
                             ELSE 2
                        END AS [statuscode],
                        [TBL].[Name] AS [Name],
                        [TBL].[MainPhoneNumber] AS [Telephone1],
                        [TBL].[Email] AS [EMailAddress1]
              FROM      [Billing].[Clients] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[AccountId] = [New].[AccountId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Current].[DeletionStateCode] = [New].[DeletionStateCode],
                    [Current].[ModifiedBy] = [New].[ModifiedBy],
                    [Current].[ModifiedOn] = [New].[ModifiedOn],
                    [Current].[OwningBusinessUnit] = [New].[OwningBusinessUnit],
                    [Current].[statecode] = [New].[statecode],
                    [Current].[statuscode] = [New].[statuscode],
                    [Current].[OwningUser] = [New].[OwningUser],
                    [Current].[Name] = [New].[Name],
                    [Current].[Telephone1] = [New].[Telephone1],
                    [Current].[EMailAddress1] = [New].[EMailAddress1]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [CreatedBy],
                     [CreatedOn],
                     [DeletionStateCode],
                     [AccountId],
                     [ImportSequenceNumber],
                     [ModifiedBy],
                     [ModifiedOn],
                     [OverriddenCreatedOn],
                     [OwningBusinessUnit],
                     [statecode],
                     [statuscode],
                     [TimeZoneRuleVersionNumber],
                     [UTCConversionTimeZoneCode],
                     [OwningUser],
                     [Name],
                     [Telephone1],
                     [EMailAddress1] )
            VALUES ( [New].[CreatedBy],
                     [New].[CreatedOn],
                     [New].[DeletionStateCode],
                     [New].[AccountId],
                     NULL,
                     [New].[ModifiedBy],
                     [New].[ModifiedOn],
                     NULL,
                     [New].[OwningBusinessUnit],
                     [New].[statecode],
                     [New].[statuscode],
                     NULL,
                     NULL,
                     [New].[OwningUser],
                     [New].[Name],
                     [New].[Telephone1],
                     [New].[EMailAddress1] );

        MERGE [DoubleGis_MSCRM].[dbo].[AccountExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [AccountId],
                        [TBL].[MainAddress] AS [dg_mainaddress],
                        [F].[ReplicationCode] AS [dg_mainfirm],
                        [TBL].[LastQualifyTime] AS [dg_lastqualifytime],
                        [TBL].[LastDisqualifyTime] AS [dg_lastdisqualifytime],
                        [TBL].[InformationSource] AS [dg_source],
                        [T].[ReplicationCode] AS [dg_territory],
                        [TBL].[PromisingValue] AS [dg_PromisingScore],
                        [TBL].[IsAdvertisingAgency] AS [dg_isadvertisingagency]
              FROM      [Billing].[Clients] AS [TBL]
                        LEFT JOIN [BusinessDirectory].[Firms] AS [F] ON [F].[Id] = [TBL].[MainFirmId]
                        JOIN [BusinessDirectory].[Territories] AS [T] ON [T].[Id] = [TBL].[TerritoryId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[AccountId] = [New].[AccountId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [dg_mainaddress] = [New].[dg_mainaddress],
                    [dg_mainfirm] = [New].[dg_mainfirm],
                    [dg_lastqualifytime] = [New].[dg_lastqualifytime],
                    [dg_lastdisqualifytime] = [New].[dg_lastdisqualifytime],
                    [dg_source] = [New].[dg_source],
                    [dg_territory] = [New].[dg_territory],
                    [dg_PromisingScore] = [New].[dg_PromisingScore],
                    [dg_isadvertisingagency] = [New].[dg_isadvertisingagency]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [dg_mainaddress],
                     [dg_mainfirm],
                     [dg_lastqualifytime],
                     [dg_lastdisqualifytime],
                     [dg_source],
                     [dg_territory],
                     [accountid],
                     [dg_PromisingScore],
                     [dg_isadvertisingagency] )
            VALUES ( [New].[dg_mainaddress],
                     [New].[dg_mainfirm],
                     [New].[dg_lastqualifytime],
                     [New].[dg_lastdisqualifytime],
                     [New].[dg_source],
                     [New].[dg_territory],
                     [New].[accountid],
                     [New].[dg_PromisingScore],
                     [New].[dg_isadvertisingagency] );

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