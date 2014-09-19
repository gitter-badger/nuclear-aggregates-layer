CREATE PROCEDURE [Billing].[ReplicateLimits]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_limitBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_limitId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[Limits] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_limitId] = [New].[Dg_limitId] )
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
                     [Dg_limitId],
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
                     [New].[Dg_limitId],
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_limitExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_limitId],
                        [TBL].[CloseDate] AS [Dg_closedate],
                        [TBL].[Amount] AS [Dg_amount],
                        [TBL].[Status] AS [Dg_status],
                        [TBL].[StartPeriodDate] AS [Dg_startperioddate],
                        [TBL].[EndPeriodDate] AS [Dg_endperioddate],
                        [LP].[ReplicationCode] AS [Dg_legalperson],
                        [BOOU].[ReplicationCode] AS [Dg_branchoffice_organizationunit],
                        [TBL].[Comment] AS [Dg_comment],
                        [ICU].[SystemUserId] AS [Dg_inspectorid]
              FROM      [Billing].[Limits] AS [TBL]
                        JOIN [Billing].[Accounts] AS [A] ON [A].[Id] = [TBL].[AccountId]
                        JOIN [Billing].[LegalPersons] AS [LP] ON [LP].[Id] = [A].[LegalPersonId]
                        JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [BOOU].[Id] = [A].[BranchOfficeOrganizationUnitId]
                        JOIN [Security].[Users] AS [IU] ON [IU].[Id] = [TBL].[InspectorCode]
                        JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [ICU] WITH ( NOEXPAND ) ON [ICU].[ErmUserAccount] = [IU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_limitId] = [New].[Dg_limitId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Dg_closedate] = [New].[Dg_closedate],
                    [Dg_amount] = [New].[Dg_amount],
                    [Dg_status] = [New].[Dg_status],
                    [Dg_startperioddate] = [New].[Dg_startperioddate],
                    [Dg_endperioddate] = [New].[Dg_endperioddate],
                    [Dg_legalperson] = [New].[Dg_legalperson],
                    [Dg_branchoffice_organizationunit] = [New].[Dg_branchoffice_organizationunit],
                    [Dg_comment] = [New].[Dg_comment],
                    [Dg_inspectorid] = [New].[Dg_inspectorid]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [Dg_limitId],
                     [Dg_closedate],
                     [Dg_amount],
                     [Dg_status],
                     [Dg_startperioddate],
                     [Dg_endperioddate],
                     [Dg_legalperson],
                     [Dg_branchoffice_organizationunit],
                     [Dg_comment],
                     [Dg_inspectorid] )
            VALUES ( [New].[Dg_limitId],
                     [New].[Dg_closedate],
                     [New].[Dg_amount],
                     [New].[Dg_status],
                     [New].[Dg_startperioddate],
                     [New].[Dg_endperioddate],
                     [New].[Dg_legalperson],
                     [New].[Dg_branchoffice_organizationunit],
                     [New].[Dg_comment],
                     [New].[Dg_inspectorid] );

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