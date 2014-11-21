CREATE PROCEDURE [Billing].[ReplicateLegalPersons]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_legalpersonBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_legalpersonId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[LegalPersons] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_legalpersonId] = [New].[Dg_legalpersonId] )
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
                     [Dg_legalpersonId],
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
                     [New].[Dg_legalpersonId],
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_legalpersonExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_legalpersonId],
                        [TBL].[Inn] AS [Dg_inn],
                        [TBL].[Kpp] AS [Dg_kpp],
                        [TBL].[ShortName] AS [Dg_name],
                        [TBL].[LegalName] AS [Dg_legalname],
                        [C].[ReplicationCode] AS [dg_account_legalperson],
                        [TBL].[PassportSeries] AS [dg_passportseries],
                        [TBL].[PassportNumber] AS [dg_passportnumber],
                        [TBL].[LegalAddress] AS [dg_legaladdress],
                        [TBL].[LegalPersonTypeEnum] + 1 AS [dg_legalpersontype]
              FROM      [Billing].[LegalPersons] AS [TBL]
                        JOIN [Billing].[Clients] AS [C] ON [C].[Id] = [TBL].[ClientId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_legalpersonId] = [New].[Dg_legalpersonId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Dg_inn] = [New].[Dg_inn],
                    [Dg_kpp] = [New].[Dg_kpp],
                    [Dg_name] = [New].[Dg_name],
                    [Dg_legalname] = [New].[Dg_legalname],
                    [dg_account_legalperson] = [New].[dg_account_legalperson],
                    [dg_passportseries] = [New].[dg_passportseries],
                    [dg_passportnumber] = [New].[dg_passportnumber],
                    [dg_legaladdress] = [New].[dg_legaladdress],
                    [dg_legalpersontype] = [New].[dg_legalpersontype]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [dg_inn],
                     [dg_kpp],
                     [dg_legalpersonId],
                     [dg_name],
                     [dg_legalname],
                     [dg_account_legalperson],
                     [dg_passportseries],
                     [dg_legaladdress],
                     [dg_legalpersontype],
                     [dg_passportnumber] )
            VALUES ( [New].[dg_inn],
                     [New].[dg_kpp],
                     [New].[dg_legalpersonId],
                     [New].[dg_name],
                     [New].[dg_legalname],
                     [New].[dg_account_legalperson],
                     [New].[dg_passportseries],
                     [New].[dg_legaladdress],
                     [New].[dg_legalpersontype],
                     [New].[dg_passportnumber] );

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