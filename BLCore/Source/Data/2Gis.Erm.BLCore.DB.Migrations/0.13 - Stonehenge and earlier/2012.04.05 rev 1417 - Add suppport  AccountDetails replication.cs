using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1417, "Добавление колонки ReplicationCode в OperationTypes," +
                        " инфраструктура для репликации AccountDetail и OperationType в MSCRM")]
    // ReSharper disable InconsistentNaming
    public class Migration_1417_AccountDetail_OperationType_Replication : IContextedMigration<IMigrationContext>
        // ReSharper restore InconsistentNaming
    { 
        #region SQL statement

        private const String CreateReplicateOperationTypeSPTemplate = @"
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
	FROM [Billing].[OperationTypes] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_operationtypeBase] WHERE [Dg_operationtypeId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_operationtypeBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_operationtypeId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
		   ,[OwningUser]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
		   )
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[OT].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OT].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[OT].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			@OwnerUserId,
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL				-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_operationtypeExtensionBase]
			([Dg_operationtypeId]
           ,[Dg_name]
           ,[Dg_IsPlus])
		SELECT 
			 [OT].[ReplicationCode]
			,[OT].[Name]
			,[OT].[IsPlus]
		FROM [Billing].[OperationTypes] AS [OT]
		WHERE [OT].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [OT].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OT].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [OT].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OT].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_operationtypeBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
		

		UPDATE [CRMAC]
			SET 
			 [Dg_name] = [OT].[Name]
			,[Dg_IsPlus] = [OT].[IsPlus]
		FROM [{0}].[dbo].[Dg_operationtypeExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[OperationTypes] AS [OT] ON [CRMAC].[Dg_operationtypeId] = [OT].[ReplicationCode] AND [OT].[Id] = @Id;
	END;
	
	RETURN 1;";
            
        private const String CreateReplicateAccountDetailSPTemplate = @"
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
	FROM [Billing].[AccountDetails] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

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


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_accountdetailBase] WHERE [Dg_accountdetailId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_accountdetailBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_accountdetailId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
		   ,[OwningBusinessUnit]
		   ,[OwningUser]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
		   )
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[AD].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [AD].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[AD].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId), -- <ModifiedBy, uniqueidentifier,>
			[AD].[ModifiedOn],	-- <ModifiedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			@OwnerUserId,-- <OwningBusinessUnit, uniqueidentifier,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [AD].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [AD].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL				-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[AccountDetails] AS [AD]
		WHERE [AD].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_accountdetailExtensionBase]
			([Dg_accountdetailId]
           ,[Dg_description]
           ,[Dg_amount]
           ,[Dg_comment]
           ,[Dg_transactiondate]
           ,[Dg_account]
           ,[Dg_operationtype])
		SELECT
			[AD].[ReplicationCode]
			,[AD].[Description]
			,[AD].[Amount]
			,[AD].[Comment]
			,[AD].[TransactionDate]
			,(SELECT [AC].ReplicationCode FROM [Billing].[Accounts] AS [AC] WHERE [AC].Id = [AD].[AccountId])
			,(SELECT [OT].ReplicationCode FROM [Billing].[OperationTypes] AS [OT] WHERE [OT].Id = [AD].[OperationTypeId])
		FROM [Billing].[AccountDetails] AS [AD]
		WHERE [AD].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMAC]
			SET 
			  [DeletionStateCode] = CASE WHEN [AD].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [AD].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [AD].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [AD].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_accountdetailBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
		

		UPDATE [CRMAC]
		SET 
			 [Dg_description] = [AD].[Description]
			,[Dg_amount] = [AD].[Amount]
			,[Dg_comment] = [AD].[Comment]
			,[Dg_transactiondate] = [AD].[TransactionDate]
			,[Dg_account] = (SELECT [AC].ReplicationCode FROM [Billing].[Accounts] AS [AC] WHERE [AC].Id = [AD].[AccountId])
			,[Dg_operationtype] = (SELECT [OT].ReplicationCode FROM [Billing].[OperationTypes] AS [OT] WHERE [OT].Id = [AD].[OperationTypeId])
		FROM [{0}].[dbo].[Dg_accountdetailExtensionBase] AS [CRMAC]
			INNER JOIN [Billing].[AccountDetails] AS [AD] ON [CRMAC].[Dg_accountdetailId] = [AD].[ReplicationCode] AND [AD].[Id] = @Id;
	END;
	
	RETURN 1;";

            private const String UpdateReplicateEverythingSPTemplate = @"
    SET NOCOUNT ON

    BEGIN TRANSACTION TxReplicateEverything

    DECLARE @id INT;
    DECLARE @counter INT = 0;

    DECLARE organizationUnitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OrganizationUnits OU
			LEFT JOIN [{0}].[dbo].[Dg_organizationunitBase] MOU ON OU.ReplicationCode=MOU.[Dg_organizationunitId]
			WHERE MOU.ModifiedOn IS NULL OR OU.ModifiedOn!=MOU.ModifiedOn
    OPEN organizationUnitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM organizationUnitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация отделения организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE Billing.[ReplicateOrganizationUnit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE organizationUnitCursor;
    DEALLOCATE organizationUnitCursor;

    DECLARE currencyCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from [Billing].[Currencies] C
			LEFT JOIN [{0}].[dbo].[Dg_currencyBase] MC ON C.ReplicationCode=MC.[Dg_currencyId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn
    OPEN currencyCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM currencyCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация валюты с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE Billing.[ReplicateCurrency] @id;
          SET @counter = @counter + 1;
    END
    CLOSE currencyCursor;
    DEALLOCATE currencyCursor;

	DECLARE categoryCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Categories C
			LEFT JOIN [{0}].[dbo].[Dg_categoryBase] MC ON C.ReplicationCode=MC.[Dg_categoryId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN categoryCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM categoryCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация рубрик с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE BusinessDirectory.[ReplicateCategory] @id;
          SET @counter = @counter + 1;
    END
    CLOSE categoryCursor;
    DEALLOCATE categoryCursor;


    DECLARE territoryCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Territories T
			LEFT JOIN [{0}].[dbo].[Dg_territoryBase] MT ON T.ReplicationCode=MT.[Dg_territoryId]
			WHERE MT.ModifiedOn IS NULL OR T.ModifiedOn!=MT.ModifiedOn

    OPEN territoryCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM territoryCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация территории с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE BusinessDirectory.[ReplicateTerritory] @id;
          SET @counter = @counter + 1;
    END
    CLOSE territoryCursor;
    DEALLOCATE territoryCursor;

	DECLARE @DeleteIndex BIT
	SET @DeleteIndex=0
	
	IF(EXISTS(SELECT TOP 1(Id) from Billing.Clients C
			LEFT JOIN [{0}].[dbo].[AccountBase] MC ON C.ReplicationCode=MC.[AccountId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn)
		AND
		EXISTS(SELECT TOP 1(Id) from BusinessDirectory.Firms F
			LEFT JOIN [{0}].[dbo].[Dg_firmBase] MF ON F.ReplicationCode=MF.[Dg_firmId]
			WHERE MF.ModifiedOn IS NULL OR F.ModifiedOn!=MF.ModifiedOn))
	BEGIN
		PRINT 'Удаление отношения между клинетами и фирмами для успешного прохождения репликации в обход циклических связей'
		SET @DeleteIndex=1
		ALTER TABLE [{0}].[dbo].[AccountExtensionBase] DROP CONSTRAINT [dg_dg_firm_account];
	END

	DECLARE clientCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Clients C
			LEFT JOIN [{0}].[dbo].[AccountBase] MC ON C.ReplicationCode=MC.[AccountId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN clientCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM clientCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация клиента с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateClient] @id;
          SET @counter = @counter + 1;
    END
    CLOSE clientCursor;
    DEALLOCATE clientCursor;
    
    
   DECLARE firmCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Firms F
			LEFT JOIN [{0}].[dbo].[Dg_firmBase] MF ON F.ReplicationCode=MF.[Dg_firmId]
			WHERE MF.ModifiedOn IS NULL OR F.ModifiedOn!=MF.ModifiedOn

    OPEN firmCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM firmCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация фирмы с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [BusinessDirectory].[ReplicateFirm] @id;
          SET @counter = @counter + 1;
    END
    CLOSE firmCursor;
    DEALLOCATE firmCursor;
	
	IF(@DeleteIndex=1)
	BEGIN
		PRINT 'Восстановление отношения между клинетами и фирмами после репликации клиентов и фирм'
		ALTER TABLE [{0}].[dbo].[AccountExtensionBase]  WITH NOCHECK ADD  CONSTRAINT [dg_dg_firm_account] FOREIGN KEY([dg_mainfirm])
		REFERENCES [{0}].[dbo].[Dg_firmBase] ([Dg_firmId])
		ALTER TABLE [{0}].[dbo].[AccountExtensionBase] CHECK CONSTRAINT [dg_dg_firm_account]
    END

    
    DECLARE firmAddressCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.FirmAddresses FA
			LEFT JOIN [{0}].[dbo].[Dg_firmaddressBase] MFA ON FA.ReplicationCode=MFA.[Dg_firmaddressId]
			WHERE MFA.ModifiedOn IS NULL OR FA.ModifiedOn!=MFA.ModifiedOn

    OPEN firmAddressCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM firmAddressCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация адреса фирмы с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [BusinessDirectory].[ReplicateFirmAddress] @id;
          SET @counter = @counter + 1;
    END
    CLOSE firmAddressCursor;
    DEALLOCATE firmAddressCursor;
    

	DECLARE contactCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Contacts C
			LEFT JOIN [{0}].[dbo].[ContactBase] MC ON C.ReplicationCode=MC.[ContactId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN contactCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM contactCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация контакта с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateContact] @id;
          SET @counter = @counter + 1;
    END
    CLOSE contactCursor;
    DEALLOCATE contactCursor;

    DECLARE positionCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Positions P
			LEFT JOIN [{0}].[dbo].[Dg_positionBase] MP ON P.ReplicationCode=MP.[Dg_positionId]
			WHERE MP.ModifiedOn IS NULL OR P.ModifiedOn!=MP.ModifiedOn
    OPEN positionCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM positionCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация позиции прайса с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicatePosition] @id;
          SET @counter = @counter + 1;
    END
    CLOSE positionCursor;
    DEALLOCATE positionCursor;

    DECLARE branchOfficeCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.BranchOffices BO
			LEFT JOIN [{0}].[dbo].[Dg_branchofficeBase] MBO ON BO.ReplicationCode=MBO.[Dg_branchofficeId]
			WHERE MBO.ModifiedOn IS NULL OR BO.ModifiedOn!=MBO.ModifiedOn
    OPEN branchOfficeCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM branchOfficeCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация юр.лица организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBranchOffice] @id;
          SET @counter = @counter + 1;
    END
    CLOSE branchOfficeCursor;
    DEALLOCATE branchOfficeCursor;

    DECLARE branchOfficeOrganizationUnitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.BranchOfficeOrganizationUnits BOOU
			LEFT JOIN [{0}].[dbo].[Dg_branchoffice_organizationunitBase] MBOOU ON BOOU.ReplicationCode=MBOOU.[Dg_branchoffice_organizationunitId]
			WHERE MBOOU.ModifiedOn IS NULL OR BOOU.ModifiedOn!=MBOOU.ModifiedOn
    OPEN branchOfficeOrganizationUnitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM branchOfficeOrganizationUnitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация юр.лица отделения организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBranchOfficeOrganizationUnit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE branchOfficeOrganizationUnitCursor;
    DEALLOCATE branchOfficeOrganizationUnitCursor;

    DECLARE legalPersonCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.LegalPersons LP
			LEFT JOIN [{0}].[dbo].[Dg_legalpersonBase] MLP ON LP.ReplicationCode=MLP.[Dg_legalpersonId]
			WHERE MLP.ModifiedOn IS NULL OR LP.ModifiedOn!=MLP.ModifiedOn
    OPEN legalPersonCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM legalPersonCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация юр.лица с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateLegalPerson] @id;
          SET @counter = @counter + 1;
    END
    CLOSE legalPersonCursor;
    DEALLOCATE legalPersonCursor;

    DECLARE accountCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Accounts A
			LEFT JOIN [{0}].[dbo].[Dg_account] MA ON A.ReplicationCode=MA.[Dg_accountId]
			WHERE MA.ModifiedOn IS NULL OR A.ModifiedOn!=MA.ModifiedOn
    OPEN accountCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM accountCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация лицевого счета с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateAccount] @id;
          SET @counter = @counter + 1;
    END
    CLOSE accountCursor;
    DEALLOCATE accountCursor;
    
    DECLARE operationTypeCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OperationTypes OT
			LEFT JOIN [{0}].[dbo].[Dg_operationtype] MA ON OT.ReplicationCode=MA.[Dg_operationtypeId]
			WHERE MA.ModifiedOn IS NULL OR OT.ModifiedOn!=MA.ModifiedOn
    OPEN operationTypeCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM operationTypeCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация типа операции по лицевому счету с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOperationType] @id;
          SET @counter = @counter + 1;
    END
    CLOSE operationTypeCursor;
    DEALLOCATE operationTypeCursor;
    
    DECLARE accountDetailCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.AccountDetails AD
			LEFT JOIN [{0}].[dbo].[Dg_accountdetail] MA ON AD.ReplicationCode=MA.[Dg_accountdetailId]
			WHERE MA.ModifiedOn IS NULL OR AD.ModifiedOn!=MA.ModifiedOn
    OPEN accountDetailCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM accountDetailCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация операции по лицевому счету с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateAccountDetail] @id;
          SET @counter = @counter + 1;
    END
    CLOSE accountDetailCursor;
    DEALLOCATE accountDetailCursor;

    DECLARE dealCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Deals D
			LEFT JOIN [{0}].[dbo].[OpportunityBase] MO ON D.ReplicationCode=MO.[OpportunityId]
			WHERE MO.ModifiedOn IS NULL OR D.ModifiedOn!=MO.ModifiedOn
    OPEN dealCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM dealCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация сделки с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateDeal] @id;
          SET @counter = @counter + 1;
    END
    CLOSE dealCursor;
    DEALLOCATE dealCursor;

    DECLARE limitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Limits L
			LEFT JOIN [{0}].[dbo].[Dg_limitBase] ML ON L.ReplicationCode=ML.[Dg_limitId]
			WHERE ML.ModifiedOn IS NULL OR L.ModifiedOn!=ML.ModifiedOn
    OPEN limitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM limitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация лимита с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateLimit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE limitCursor;
    DEALLOCATE limitCursor;

    DECLARE orderCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Orders O
			LEFT JOIN [{0}].[dbo].[Dg_orderBase] MO ON O.ReplicationCode=MO.[Dg_orderId]
			WHERE MO.ModifiedOn IS NULL OR O.ModifiedOn!=MO.ModifiedOn
    OPEN orderCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM orderCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация заказа с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOrder] @id;
          SET @counter = @counter + 1;
    END
    CLOSE orderCursor;
    DEALLOCATE orderCursor;

    DECLARE orderPositionCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OrderPositions OP
			LEFT JOIN [{0}].[dbo].[Dg_orderpositionBase] MOP ON OP.ReplicationCode=MOP.[Dg_orderpositionId]
			WHERE MOP.ModifiedOn IS NULL OR OP.ModifiedOn!=MOP.ModifiedOn
    OPEN orderPositionCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM orderPositionCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация позиции заказа с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOrderPosition] @id;
          SET @counter = @counter + 1;
    END
    CLOSE orderPositionCursor;
    DEALLOCATE orderPositionCursor;

    DECLARE bargainCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Bargains B
			LEFT JOIN [{0}].[dbo].[Dg_bargain] MB ON B.ReplicationCode=MB.[Dg_bargainId]
			WHERE MB.ModifiedOn IS NULL OR B.ModifiedOn!=MB.ModifiedOn
    OPEN bargainCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM bargainCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT 'Репликация договора с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBargain] @id;
          SET @counter = @counter + 1;
    END
    CLOSE bargainCursor;
    DEALLOCATE bargainCursor;

    PRINT @counter;

    COMMIT TRANSACTION TxReplicateEverything
    SET NOCOUNT OFF";
        #endregion

        public void Apply(IMigrationContext context)
        {
           
            using (var scope = new TransactionScope())
            {
                AddReplicationCodeColumnToOperationsType(context);
                CreateReplicateOperationTypeSP(context);
                CreateReplicateAccountDetailSP(context);
                UpdateReplicateEverythingSP(context);

                scope.Complete();
            }
        }
        
        private const String ReplicationCodecolumnName = "ReplicationCode";
        private readonly IList<Tuple<int, String>> _ermIdToReplicationCodeMap = new List<Tuple<int, string>>(){
                                                new Tuple<int, string>(1, "9237D7A7-417C-4C84-BACF-09B9AE08CCE5"),
                                                new Tuple<int, string>(2, "6572AE89-8D27-4A3A-8292-BA7465390466"),
                                                new Tuple<int, string>(3, "E1205DE7-52EC-4E74-B3DC-A720E5C35110"),
                                                new Tuple<int, string>(4, "01794226-B57F-4D6E-9D02-D360D7077E2A"),
                                                new Tuple<int, string>(5, "DF510055-AA22-4E26-AB95-CEA71126C6DF"),
                                                new Tuple<int, string>(6, "E4CC566E-4540-4C8F-9A45-B31C84D716F9"),
                                                new Tuple<int, string>(7, "5F26B5F3-85FA-4897-89D3-4F48DD9E3805"),
                                                new Tuple<int, string>(8, "9C33C6E0-DAAB-4F3F-A6F9-D24A3E2BC11C"),
                                                new Tuple<int, string>(9, "579E8C61-B255-4B0B-A89B-913773EE5DA6"),
                                                new Tuple<int, string>(10, "C1071F46-B11E-40E8-B1AE-FD92D0FFDA7D"),
                                                new Tuple<int, string>(11, "EFEDAF21-737F-4EC9-9281-A0B7995A89EB"),
                                                new Tuple<int, string>(12, "62F39C5B-911D-4D2B-87BE-83C7739F945A"),
                                                new Tuple<int, string>(13, "E1732710-7092-49F7-BE98-C8BA923483CF"),
                                                new Tuple<int, string>(14, "4951C1AC-9C3C-4DD8-A715-9F786E35AC41"),
                                                new Tuple<int, string>(15, "9C653CF7-177A-4577-8B88-E8E320E8705D"),
                                                new Tuple<int, string>(16, "A9AB64FB-0FD4-4C5C-B45A-2602620D18D5"),
                                                new Tuple<int, string>(17, "D3094617-DC2E-4F89-86B7-7715979A37E6"),
        };

        private void AddReplicationCodeColumnToOperationsType(IMigrationContext context)
        {
            var operationTypesTable = context.Database.Tables[ErmTableNames.OperationTypes.Name, ErmTableNames.OperationTypes.Schema];
            if (operationTypesTable == null)
            {
                throw new InvalidOperationException("БД " + context.Database.Name + " не содержит таблицы " + ErmTableNames.OperationTypes);
            }

            if (operationTypesTable.Columns.Contains(ReplicationCodecolumnName))
            {
                // колонка уже есть, добавлять ничего не надо
                return;
            }

            var newColumn = new Column(operationTypesTable, ReplicationCodecolumnName, DataType.UniqueIdentifier);
            newColumn.Nullable = true;
            newColumn.Create();

            var sb = new StringBuilder();
            foreach (var mapEntry in _ermIdToReplicationCodeMap)
            {
                sb.AppendFormat("UPDATE {0} SET {1} = '{2}' WHERE Id = {3};\n", ErmTableNames.OperationTypes, ReplicationCodecolumnName, mapEntry.Item2, mapEntry.Item1);
            }

            context.Connection.ExecuteNonQuery(sb.ToString());

            newColumn.Nullable = false;
            newColumn.Alter();
        }

        private void CreateReplicateOperationTypeSP(IMigrationContext context)
        {
            if (context.Database.StoredProcedures.Contains(ErmStoredProcedures.ReplicateOperationType.Name, ErmStoredProcedures.ReplicateOperationType.Schema))
            {
                // процедура уже есть
                return;
            }

            var sp = new StoredProcedure(context.Database, ErmStoredProcedures.ReplicateOperationType.Name, ErmStoredProcedures.ReplicateOperationType.Schema);
            sp.TextMode = false;
            sp.AnsiNullsStatus = false;
            sp.QuotedIdentifierStatus = false;
            var param = new StoredProcedureParameter(sp, "@ID", DataType.Int){DefaultValue = "NULL"};
            sp.Parameters.Add(param);
            sp.TextBody = String.Format(CreateReplicateOperationTypeSPTemplate);
            sp.Create();
        }
        
        private void CreateReplicateAccountDetailSP(IMigrationContext context)
        {
            if (context.Database.StoredProcedures.Contains(ErmStoredProcedures.ReplicateAccountDetail.Name, ErmStoredProcedures.ReplicateAccountDetail.Schema))
            {
                // процедура уже есть
                return;
            }

            var sp = new StoredProcedure(context.Database, ErmStoredProcedures.ReplicateAccountDetail.Name, ErmStoredProcedures.ReplicateAccountDetail.Schema);
            sp.TextMode = false;
            sp.AnsiNullsStatus = false;
            sp.QuotedIdentifierStatus = false;
            var param = new StoredProcedureParameter(sp, "@ID", DataType.Int) { DefaultValue = "NULL" };
            sp.Parameters.Add(param);
            sp.TextBody = String.Format(CreateReplicateAccountDetailSPTemplate, context.CrmDatabaseName);
            sp.Create();
        }

        private void UpdateReplicateEverythingSP(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures[ErmStoredProcedures.ReplicateEverything.Name, ErmStoredProcedures.ReplicateEverything.Schema];
            sp.TextBody = String.Format(UpdateReplicateEverythingSPTemplate, context.CrmDatabaseName);
            sp.Alter();
        }

        public void Revert(IMigrationContext context)
        {
            throw new NotImplementedException("Откат не поддерживается");
        }
    }
}
