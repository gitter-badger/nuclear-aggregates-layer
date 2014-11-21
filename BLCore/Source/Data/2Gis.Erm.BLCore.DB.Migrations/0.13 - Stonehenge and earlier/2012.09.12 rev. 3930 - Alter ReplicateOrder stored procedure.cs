using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3930, "Изменение хранимой процедуры ReplicateOrder - добавлена в репликацию колонка Comment (комментарий к расторжению заказа)")]
    public sealed class Migration3930 : TransactedMigration
    {
        #region SP Text

        private const string StoredProcedureTextTemplate = @"
    SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;

	SET XACT_ABORT ON;

	DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserDomainName NVARCHAR(250);
    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
    DECLARE @ModifiedByUserDomainName NVARCHAR(250);
	DECLARE @InspectorUserDomainName NVARCHAR(250);
	DECLARE @InspectorUserId UNIQUEIDENTIFIER;
	
	DECLARE @OwnerUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserId UNIQUEIDENTIFIER;
	DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @InspectorUserDomainName = [I].[Account],
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[Orders] AS [TBL]
        INNER JOIN [Billing].[OrderExtensions] AS [TBLEX] ON [TBLEX].[Id] = [TBL].[Id]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
        LEFT OUTER JOIN [Security].[Users] AS [I] ON [I].[Id] = [TBLEX].[InspectorCode]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

	-- get inspector user CRM UserId
    IF (@InspectorUserDomainName IS NOT NULL)
	    SELECT @InspectorUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @InspectorUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_orderBase] WHERE [Dg_orderId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN
	
		INSERT INTO [{0}].[dbo].[Dg_orderBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_orderId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OverriddenCreatedOn]
           ,[OwningBusinessUnit]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
           ,[OwningUser])
		SELECT
           @CreatedByUserId				-- <CreatedBy, uniqueidentifier,>
           ,[Orders].[CreatedOn]		-- <CreatedOn, datetime,>
			,CASE WHEN [Orders].[IsDeleted] = 1 THEN 2 ELSE 0 END -- <DeletionStateCode, int,>
           ,[Orders].[ReplicationCode]	-- <Dg_orderId, uniqueidentifier,>
           ,NULL						-- <ImportSequenceNumber, int,>
		   ,ISNULL(@ModifiedByUserId, @CreatedByUserId)				-- <ModifiedBy, uniqueidentifier,>
		   ,[Orders].[ModifiedOn]		-- <ModifiedOn, datetime,>
           ,NULL						-- <OverriddenCreatedOn, datetime,>
           ,@OwnerUserBusinessUnitId	-- <OwningBusinessUnit, uniqueidentifier,>
			,CASE WHEN [Orders].[IsActive] = 1 THEN 0 ELSE 1 END -- <statecode, int,>
			,CASE WHEN [Orders].[IsActive] = 1 THEN 1 ELSE 2 END -- <statuscode, int,>
           ,NULL						-- <TimeZoneRuleVersionNumber, int,>
           ,NULL						-- <UTCConversionTimeZoneCode, int,>
           ,@OwnerUserId				-- <OwningUser, uniqueidentifier,>)
		FROM [Billing].[Orders] AS [Orders]
		WHERE [Orders].[Id] = @Id;


		INSERT INTO [{0}].[dbo].[Dg_orderExtensionBase]
           ([Dg_amounttowithdraw]
		   ,[Dg_amountwithdrawn]
           ,[Dg_begindistributiondate]
		   ,[Dg_branchofficeorganizationunit]
		   ,[Dg_opportunityid]
		   ,[Dg_dest_organizationunit]
		   ,[Dg_discountpercent]
           ,[Dg_enddistributiondatefact]
           ,[Dg_enddistributiondateplan]
		   ,[Dg_firmid]
		   ,[Dg_inspector]
           ,[Dg_isterminated]
		   ,[Dg_legalperson]
           ,[Dg_number]
           ,[Dg_regionalnumber]
           ,[Dg_orderId]
           ,[Dg_ordertype]
		   ,[Dg_payablefact]
		   ,[Dg_payableplan]
		   ,[Dg_payableprice]
           ,[Dg_source_organizationunit]
           ,[Dg_terminationreason]
		   ,[Dg_terminationcomment]
           ,[Dg_workflowstep]
		   ,[Dg_discountreason]
           ,[Dg_discountcomment]
		   ,[Dg_documentsdebt]
		   ,[Dg_documentscomment]
		   ,[Dg_account]
		   ,[Dg_signupdate]
		   ,[Dg_release_count_fact]
		   ,[Dg_release_count_plan]
		   )
		SELECT
            [OrderExtensions].[AmountToWithdraw]	-- <Dg_amounttowithdraw, decimal(23,10),>
		   ,[OrderExtensions].[AmountWithdrawn]		-- <Dg_amountwithdrawn, decimal(23,10),>
           ,[Orders].[BeginDistributionDate]	-- <Dg_begindistributiondate, datetime,>
		   										-- <Dg_branchofficeorganizationunit, uniqueidentifier,>
           ,(SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].[Id] = [Orders].[BranchOfficeOrganizationUnitId])
		   										-- <Dg_opportunityid, uniqueidentifier,>
           ,(SELECT [Deals].[ReplicationCode] FROM [Billing].[Deals] WHERE [Deals].[Id] = [Orders].[DealId])
		   										-- <Dg_dest_organizationunit, uniqueidentifier,>
           ,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[DestOrganizationUnitId])
		   ,[OrderExtensions].[DiscountPercent]	-- <Dg_discountpercent, decimal(23,10),>
           ,[Orders].[EndDistributionDateFact]	-- <Dg_enddistributiondatefact, datetime,>
           ,[Orders].[EndDistributionDatePlan]	-- <Dg_enddistributiondateplan, datetime,>
		   										-- <Dg_firmid, uniqueidentifier,>
           ,(SELECT [Firms].[ReplicationCode] FROM [BusinessDirectory].[Firms] WHERE [Firms].[Id] = [Orders].[FirmId])
		   ,@InspectorUserId					-- <Dg_inspector, uniqueidentifier,>
           ,[Orders].[IsTerminated]				-- <Dg_isterminated, bit,>
		   										-- <Dg_legalperson, uniqueidentifier,>
           ,(SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [Orders].[LegalPersonId])
           ,[Orders].[Number]					-- <Dg_number, NVARCHAR(200),>
           ,[Orders].[RegionalNumber]
           ,[Orders].[ReplicationCode]			-- <Dg_orderId, uniqueidentifier,>
		   ,[OrderExtensions].[OrderType]		-- <Dg_ordertype, int,>
		   ,[OrderExtensions].[PayableFact]		-- <Dg_payablefact, decimal(23,10),>
		   ,[OrderExtensions].[PayablePlan]		-- <Dg_payableplan, decimal(23,10),>
		   ,[OrderExtensions].[PayablePrice]	-- <Dg_payableprice, decimal(23,10),>
												-- <Dg_source_organizationunit, uniqueidentifier,>
           ,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[SourceOrganizationUnitId])
           ,[OrderExtensions].[TerminationReason]	-- <Dg_terminationreason, int,>
		   ,[OrderExtensions].[Comment]				-- <Dg_terminationcomment, nvarchar(300)>
           ,[Orders].[WorkflowStepId]				-- <Dg_workflowstep, int,>	 ## mapping of workflowStepId => AttributePicklistValue.Value ##
		   ,[Orders].[DiscountReasonEnum]
           ,[Orders].[DiscountComment]
		   ,[Orders].[HasDocumentsDebt]				-- <Dg_hasdebt, smallint>
		   ,[Orders].[DocumentsComment]
		   ,(SELECT [AC].[ReplicationCode] FROM [Billing].[Accounts] AS [AC] WHERE [AC].[Id] = [Orders].[AccountId])
		   ,[Orders].[SignupDate]
		   ,[Orders].[ReleaseCountFact]
		   ,[Orders].[ReleaseCountPlan]
		FROM [Billing].[Orders]
		JOIN [Billing].[OrderExtensions] ON [OrderExtensions].Id = [Orders].Id
		WHERE [Orders].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN

		UPDATE [CRMO]
			SET 
			  [DeletionStateCode] = CASE WHEN [Orders].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [Orders].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode] = CASE WHEN [Orders].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [Orders].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[Dg_orderBase] AS [CRMO]
			INNER JOIN [Billing].[Orders] ON [CRMO].[Dg_orderId] = [Orders].[ReplicationCode] AND [Orders].[Id] = @Id;


		UPDATE [CRMO]
		   SET 
			   [Dg_amounttowithdraw] = [OrderExtensions].[AmountToWithdraw]
			  ,[Dg_amountwithdrawn]  = [OrderExtensions].[AmountWithdrawn]
			  ,[Dg_begindistributiondate] = [Orders].[BeginDistributionDate]
			  ,[Dg_branchofficeorganizationunit] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOfficeOrganizationUnits] AS [BOOU] WHERE [BOOU].[Id] = [Orders].[BranchOfficeOrganizationUnitId])
			  ,[Dg_opportunityid] = (SELECT [Deals].[ReplicationCode] FROM [Billing].[Deals] WHERE [Deals].[Id] = [Orders].[DealId])
			  ,[Dg_dest_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[DestOrganizationUnitId])
			  ,[Dg_discountpercent] = [OrderExtensions].[DiscountPercent]
			  ,[Dg_enddistributiondatefact] = [Orders].[EndDistributionDateFact]
			  ,[Dg_enddistributiondateplan] = [Orders].[EndDistributionDatePlan]
			  ,[Dg_firmid] = (SELECT [Firms].[ReplicationCode] FROM [BusinessDirectory].[Firms] WHERE [Firms].[Id] = [Orders].[FirmId])
			  ,[Dg_inspector] = @InspectorUserId
			  ,[Dg_isterminated] = [Orders].[IsTerminated]
			  ,[Dg_legalperson]  = (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [Orders].[LegalPersonId])
			  ,[Dg_number] = [Orders].[Number]
			  ,[Dg_regionalnumber] = [Orders].[RegionalNumber]
			  ,[Dg_ordertype] = [OrderExtensions].[OrderType]
			  ,[Dg_payablefact]  = [OrderExtensions].[PayableFact]				
			  ,[Dg_payableplan]  = [OrderExtensions].[PayablePlan]
		      ,[Dg_payableprice] = [OrderExtensions].[PayablePrice]	
			  ,[Dg_source_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [Orders].[SourceOrganizationUnitId])
			  ,[Dg_terminationreason] = [OrderExtensions].[TerminationReason]
			  ,[Dg_terminationcomment] = [OrderExtensions].[Comment]
			  ,[Dg_workflowstep] = [Orders].[WorkflowStepId]
			  ,[Dg_discountreason] = [Orders].[DiscountReasonEnum]
			  ,[Dg_discountcomment] = [Orders].[DiscountComment]
			  ,[Dg_documentsdebt] = [Orders].[HasDocumentsDebt]
			  ,[Dg_documentscomment] = [Orders].[DocumentsComment]
			  ,[Dg_account] = (SELECT [AC].[ReplicationCode] FROM [Billing].[Accounts] AS [AC] WHERE [AC].[Id] = [Orders].[AccountId])
			  ,[Dg_signupdate] = [Orders].[SignupDate]
			  ,[Dg_release_count_plan] = [Orders].[ReleaseCountPlan]
			  ,[Dg_release_count_fact] = [Orders].[ReleaseCountFact]
		FROM [{0}].[dbo].[Dg_orderExtensionBase] AS [CRMO]
			INNER JOIN [Billing].[Orders] ON [CRMO].[Dg_orderId] = [Orders].[ReplicationCode] AND [Orders].[Id] = @Id
			JOIN [Billing].[OrderExtensions] ON [OrderExtensions].Id = [Orders].Id
	END;

	RETURN 1;";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var spTextBody = string.Format(StoredProcedureTextTemplate, context.CrmDatabaseName);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOrder, spTextBody);
        }
    }
}
