CREATE PROCEDURE [Billing].[ReplicateOrders]
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_orderBase] AS [Current]
        USING
            ( SELECT    CASE WHEN [TBL].[IsDeleted] = 1 THEN 2 ELSE 0 END AS [DeletionStateCode],
                        [TBL].[CreatedOn] AS [CreatedOn],
                        [TBL].[ReplicationCode] AS [Dg_orderId],
                        [TBL].[ModifiedOn] AS [ModifiedOn],
                        [OCU].[BusinessUnitId] AS [OwningBusinessUnit],
                        [OCU].[SystemUserId] AS [OwningUser],
                        [CCU].[SystemUserId] AS [CreatedBy],
                        ISNULL([MCU].[SystemUserId], [CCU].[SystemUserId]) AS [ModifiedBy],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 0 ELSE 1 END AS [statecode],
                        CASE WHEN [TBL].[IsActive] = 1 THEN 1 ELSE 2 END AS [statuscode]
              FROM      [Billing].[Orders] AS [TBL]
                        JOIN [Security].[Users] AS [OU] ON [OU].[Id] = [TBL].[OwnerCode]
                        JOIN [Security].[Users] AS [CU] ON [CU].[Id] = [TBL].[CreatedBy]
                        LEFT JOIN [Security].[Users] AS [MU] ON [MU].[Id] = [TBL].[ModifiedBy]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [OCU] WITH ( NOEXPAND ) ON [OCU].[ErmUserAccount] = [OU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [CCU] WITH ( NOEXPAND ) ON [CCU].[ErmUserAccount] = [CU].[Account] COLLATE Database_Default
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] AS [MCU] WITH ( NOEXPAND ) ON [MCU].[ErmUserAccount] = [MU].[Account] COLLATE Database_Default
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_orderId] = [New].[Dg_orderId] )
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
                     [Dg_orderId],
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
                     [New].[Dg_orderId],
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

        MERGE [DoubleGis_MSCRM].[dbo].[Dg_orderExtensionBase] AS [Current]
        USING
            ( SELECT    [TBL].[ReplicationCode] AS [Dg_orderId],
                        [TBL].[AmountToWithdraw] AS [Dg_amounttowithdraw],
                        [TBL].[AmountWithdrawn] AS [Dg_amountwithdrawn],
                        [TBL].[BeginDistributionDate] AS [Dg_begindistributiondate],
                        [BOOU].[ReplicationCode] AS [Dg_branchofficeorganizationunit],
                        [D].[ReplicationCode] AS [Dg_opportunityid],
                        [DOU].[ReplicationCode] AS [Dg_dest_organizationunit],
						[TBL].[DiscountSum] AS [Dg_discountvalue],
                        [TBL].[DiscountPercent] AS [Dg_discountpercent],
                        [TBL].[EndDistributionDateFact] AS [Dg_enddistributiondatefact],
                        [TBL].[EndDistributionDatePlan] AS [Dg_enddistributiondateplan],
                        [F].[ReplicationCode] AS [Dg_firmid],
                        [CU].[SystemUserId] AS [Dg_inspector],
                        [TBL].[IsTerminated] AS [Dg_isterminated],
                        [LP].[ReplicationCode] AS [Dg_legalperson],
                        [TBL].[Number] AS [Dg_number],
                        [TBL].[RegionalNumber] AS [Dg_regionalnumber],
                        [TBL].[OrderType] AS [Dg_ordertype],
                        [TBL].[PayableFact] AS [Dg_payablefact],
                        [TBL].[PayablePlan] AS [Dg_payableplan],
                        [TBL].[PayablePrice] AS [Dg_payableprice],
                        [SOU].[ReplicationCode] AS [Dg_source_organizationunit],
                        [TBL].[TerminationReason] AS [Dg_terminationreason],
                        [TBL].[WorkflowStepId] AS [Dg_workflowstep],
                        [TBL].[DiscountReasonEnum] AS [Dg_discountreason],
                        [TBL].[DiscountComment] AS [Dg_discountcomment],
                        [TBL].[HasDocumentsDebt] AS [Dg_documentsdebt],
                        [TBL].[DocumentsComment] AS [Dg_documentscomment],
                        [A].[ReplicationCode] AS [Dg_account],
                        [TBL].[SignupDate] AS [Dg_signupdate],
                        [TBL].[ReleaseCountPlan] AS [Dg_release_count_plan],
                        [TBL].[ReleaseCountFact] AS [Dg_release_count_fact]
              FROM      [Billing].[Orders] AS [TBL]
                        JOIN [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] ON [BOOU].[Id] = [TBL].[BranchOfficeOrganizationUnitId]
                        LEFT JOIN [Billing].[Deals] AS [D] ON [D].[Id] = [TBL].[DealId]
                        JOIN [Billing].[OrganizationUnits] AS [DOU] ON [DOU].[Id] = [TBL].[DestOrganizationUnitId]
                        JOIN [BusinessDirectory].[Firms] AS [F] ON [F].[Id] = [TBL].[FirmId]
                        JOIN [Security].[Users] AS [U] ON [U].[Id] = [TBL].[InspectorCode]
                        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [CU] WITH ( NOEXPAND ) ON [CU].[ErmUserAccount] = [U].[Account] COLLATE Database_Default
                        LEFT JOIN [Billing].[LegalPersons] AS [LP] ON [LP].[Id] = [TBL].[LegalPersonId]
                        JOIN [Billing].[OrganizationUnits] AS [SOU] ON [SOU].[Id] = [TBL].[SourceOrganizationUnitId]
                        LEFT JOIN [Billing].[Accounts] AS [A] ON [A].[Id] = [TBL].[AccountId]
              WHERE     [TBL].[Id] IN ( SELECT  [Id]
                                        FROM    @Ids )
            ) AS [New]
        ON ( [Current].[Dg_orderId] = [New].[Dg_orderId] )
        WHEN MATCHED THEN
            UPDATE SET
                    [Dg_amounttowithdraw] = [New].[Dg_amounttowithdraw],
                    [Dg_amountwithdrawn] = [New].[Dg_amountwithdrawn],
                    [Dg_begindistributiondate] = [New].[Dg_begindistributiondate],
                    [Dg_branchofficeorganizationunit] = [New].[Dg_branchofficeorganizationunit],
                    [Dg_opportunityid] = [New].[Dg_opportunityid],
                    [Dg_dest_organizationunit] = [New].[Dg_dest_organizationunit],
					[Dg_discountvalue] = [New].[Dg_discountvalue],
                    [Dg_discountpercent] = [New].[Dg_discountpercent],
                    [Dg_enddistributiondatefact] = [New].[Dg_enddistributiondatefact],
                    [Dg_enddistributiondateplan] = [New].[Dg_enddistributiondateplan],
                    [Dg_firmid] = [New].[Dg_firmid],
                    [Dg_inspector] = [New].[Dg_inspector],
                    [Dg_isterminated] = [New].[Dg_isterminated],
                    [Dg_legalperson] = [New].[Dg_legalperson],
                    [Dg_number] = [New].[Dg_number],
                    [Dg_regionalnumber] = [New].[Dg_regionalnumber],
                    [Dg_ordertype] = [New].[Dg_ordertype],
                    [Dg_payablefact] = [New].[Dg_payablefact],
                    [Dg_payableplan] = [New].[Dg_payableplan],
                    [Dg_payableprice] = [New].[Dg_payableprice],
                    [Dg_source_organizationunit] = [New].[Dg_source_organizationunit],
                    [Dg_terminationreason] = [New].[Dg_terminationreason],
                    [Dg_workflowstep] = [New].[Dg_workflowstep],
                    [Dg_discountreason] = [New].[Dg_discountreason],
                    [Dg_discountcomment] = [New].[Dg_discountcomment],
                    [Dg_documentsdebt] = [New].[Dg_documentsdebt],
                    [Dg_documentscomment] = [New].[Dg_documentscomment],
                    [Dg_account] = [New].[Dg_account],
                    [Dg_signupdate] = [New].[Dg_signupdate],
                    [Dg_release_count_plan] = [New].[Dg_release_count_plan],
                    [Dg_release_count_fact] = [New].[Dg_release_count_fact]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ( [Dg_amounttowithdraw],
                     [Dg_amountwithdrawn],
                     [Dg_begindistributiondate],
                     [Dg_branchofficeorganizationunit],
                     [Dg_opportunityid],
                     [Dg_dest_organizationunit],
					 [Dg_discountvalue],
                     [Dg_discountpercent],
                     [Dg_enddistributiondatefact],
                     [Dg_enddistributiondateplan],
                     [Dg_firmid],
                     [Dg_inspector],
                     [Dg_isterminated],
                     [Dg_legalperson],
                     [Dg_number],
                     [Dg_regionalnumber],
                     [Dg_orderId],
                     [Dg_ordertype],
                     [Dg_payablefact],
                     [Dg_payableplan],
                     [Dg_payableprice],
                     [Dg_source_organizationunit],
                     [Dg_terminationreason],
                     [Dg_workflowstep],
                     [Dg_discountreason],
                     [Dg_discountcomment],
                     [Dg_documentsdebt],
                     [Dg_documentscomment],
                     [Dg_account],
                     [Dg_signupdate],
                     [Dg_release_count_fact],
                     [Dg_release_count_plan] )
            VALUES ( [New].[Dg_amounttowithdraw],
                     [New].[Dg_amountwithdrawn],
                     [New].[Dg_begindistributiondate],
                     [New].[Dg_branchofficeorganizationunit],
                     [New].[Dg_opportunityid],
                     [New].[Dg_dest_organizationunit],
					 [New].[Dg_discountvalue],
                     [New].[Dg_discountpercent],
                     [New].[Dg_enddistributiondatefact],
                     [New].[Dg_enddistributiondateplan],
                     [New].[Dg_firmid],
                     [New].[Dg_inspector],
                     [New].[Dg_isterminated],
                     [New].[Dg_legalperson],
                     [New].[Dg_number],
                     [New].[Dg_regionalnumber],
                     [New].[Dg_orderId],
                     [New].[Dg_ordertype],
                     [New].[Dg_payablefact],
                     [New].[Dg_payableplan],
                     [New].[Dg_payableprice],
                     [New].[Dg_source_organizationunit],
                     [New].[Dg_terminationreason],
                     [New].[Dg_workflowstep],
                     [New].[Dg_discountreason],
                     [New].[Dg_discountcomment],
                     [New].[Dg_documentsdebt],
                     [New].[Dg_documentscomment],
                     [New].[Dg_account],
                     [New].[Dg_signupdate],
                     [New].[Dg_release_count_fact],
                     [New].[Dg_release_count_plan] );

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