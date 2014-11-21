-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Shared].[ReplicateEverything]
AS

    SET NOCOUNT ON

--    BEGIN TRANSACTION TxReplicateEverything

    DECLARE @id bigint;
    DECLARE @counter INT = 0;

    DECLARE organizationUnitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OrganizationUnits OU
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_organizationunitBase] MOU ON OU.ReplicationCode=MOU.[Dg_organizationunitId]
			WHERE MOU.ModifiedOn IS NULL OR OU.ModifiedOn!=MOU.ModifiedOn
    OPEN organizationUnitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM organizationUnitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация отделения организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE Billing.[ReplicateOrganizationUnit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE organizationUnitCursor;
    DEALLOCATE organizationUnitCursor;

    DECLARE currencyCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from [Billing].[Currencies] C
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_currencyBase] MC ON C.ReplicationCode=MC.[Dg_currencyId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn
    OPEN currencyCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM currencyCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация валюты с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE Billing.[ReplicateCurrency] @id;
          SET @counter = @counter + 1;
    END
    CLOSE currencyCursor;
    DEALLOCATE currencyCursor;

	DECLARE categoryCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Categories C
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_categoryBase] MC ON C.ReplicationCode=MC.[Dg_categoryId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN categoryCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM categoryCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация рубрик с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE BusinessDirectory.[ReplicateCategory] @id;
          SET @counter = @counter + 1;
    END
    CLOSE categoryCursor;
    DEALLOCATE categoryCursor;


    DECLARE territoryCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Territories T
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] MT ON T.ReplicationCode=MT.[Dg_territoryId]
			WHERE MT.ModifiedOn IS NULL OR T.ModifiedOn!=MT.ModifiedOn

    OPEN territoryCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM territoryCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация территории с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE BusinessDirectory.[ReplicateTerritory] @id;
          SET @counter = @counter + 1;
    END
    CLOSE territoryCursor;
    DEALLOCATE territoryCursor;

	DECLARE @DeleteIndex BIT
	SET @DeleteIndex=0
	
	IF(EXISTS(SELECT TOP 1(Id) from Billing.Clients C
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[AccountBase] MC ON C.ReplicationCode=MC.[AccountId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn)
		AND
		EXISTS(SELECT TOP 1(Id) from BusinessDirectory.Firms F
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_firmBase] MF ON F.ReplicationCode=MF.[Dg_firmId]
			WHERE MF.ModifiedOn IS NULL OR F.ModifiedOn!=MF.ModifiedOn))
	BEGIN
		PRINT N'Удаление отношения между клинетами и фирмами для успешного прохождения репликации в обход циклических связей'
		SET @DeleteIndex=1
		ALTER TABLE [DoubleGis_MSCRM].[dbo].[AccountExtensionBase] DROP CONSTRAINT [dg_dg_firm_account];
	END

	DECLARE clientCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Clients C
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[AccountBase] MC ON C.ReplicationCode=MC.[AccountId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN clientCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM clientCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация клиента с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateClient] @id;
          SET @counter = @counter + 1;
    END
    CLOSE clientCursor;
    DEALLOCATE clientCursor;
    
    
   DECLARE firmCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.Firms F
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_firmBase] MF ON F.ReplicationCode=MF.[Dg_firmId]
			WHERE MF.ModifiedOn IS NULL OR F.ModifiedOn!=MF.ModifiedOn

    OPEN firmCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM firmCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация фирмы с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [BusinessDirectory].[ReplicateFirm] @id;
          SET @counter = @counter + 1;
    END
    CLOSE firmCursor;
    DEALLOCATE firmCursor;
	
	IF(@DeleteIndex=1)
	BEGIN
		PRINT N'Восстановление отношения между клинетами и фирмами после репликации клиентов и фирм'
		ALTER TABLE [DoubleGis_MSCRM].[dbo].[AccountExtensionBase]  WITH NOCHECK ADD  CONSTRAINT [dg_dg_firm_account] FOREIGN KEY([dg_mainfirm])
		REFERENCES [DoubleGis_MSCRM].[dbo].[Dg_firmBase] ([Dg_firmId])
		ALTER TABLE [DoubleGis_MSCRM].[dbo].[AccountExtensionBase] CHECK CONSTRAINT [dg_dg_firm_account]
    END

    
    DECLARE firmAddressCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from BusinessDirectory.FirmAddresses FA
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_firmaddressBase] MFA ON FA.ReplicationCode=MFA.[Dg_firmaddressId]
			WHERE MFA.ModifiedOn IS NULL OR FA.ModifiedOn!=MFA.ModifiedOn

    OPEN firmAddressCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM firmAddressCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация адреса фирмы с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [BusinessDirectory].[ReplicateFirmAddress] @id;
          SET @counter = @counter + 1;
    END
    CLOSE firmAddressCursor;
    DEALLOCATE firmAddressCursor;
    

	DECLARE contactCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Contacts C
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[ContactBase] MC ON C.ReplicationCode=MC.[ContactId]
			WHERE MC.ModifiedOn IS NULL OR C.ModifiedOn!=MC.ModifiedOn

    OPEN contactCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM contactCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация контакта с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateContact] @id;
          SET @counter = @counter + 1;
    END
    CLOSE contactCursor;
    DEALLOCATE contactCursor;

    DECLARE positionCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Positions P
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_positionBase] MP ON P.ReplicationCode=MP.[Dg_positionId]
			WHERE MP.ModifiedOn IS NULL OR P.ModifiedOn!=MP.ModifiedOn
    OPEN positionCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM positionCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация позиции прайса с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicatePosition] @id;
          SET @counter = @counter + 1;
    END
    CLOSE positionCursor;
    DEALLOCATE positionCursor;

    DECLARE branchOfficeCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.BranchOffices BO
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_branchofficeBase] MBO ON BO.ReplicationCode=MBO.[Dg_branchofficeId]
			WHERE MBO.ModifiedOn IS NULL OR BO.ModifiedOn!=MBO.ModifiedOn
    OPEN branchOfficeCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM branchOfficeCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация юр.лица организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBranchOffice] @id;
          SET @counter = @counter + 1;
    END
    CLOSE branchOfficeCursor;
    DEALLOCATE branchOfficeCursor;

    DECLARE branchOfficeOrganizationUnitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.BranchOfficeOrganizationUnits BOOU
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_branchoffice_organizationunitBase] MBOOU ON BOOU.ReplicationCode=MBOOU.[Dg_branchoffice_organizationunitId]
			WHERE MBOOU.ModifiedOn IS NULL OR BOOU.ModifiedOn!=MBOOU.ModifiedOn
    OPEN branchOfficeOrganizationUnitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM branchOfficeOrganizationUnitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация юр.лица отделения организации с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBranchOfficeOrganizationUnit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE branchOfficeOrganizationUnitCursor;
    DEALLOCATE branchOfficeOrganizationUnitCursor;

    DECLARE legalPersonCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.LegalPersons LP
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_legalpersonBase] MLP ON LP.ReplicationCode=MLP.[Dg_legalpersonId]
			WHERE MLP.ModifiedOn IS NULL OR LP.ModifiedOn!=MLP.ModifiedOn
    OPEN legalPersonCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM legalPersonCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация юр.лица с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateLegalPerson] @id;
          SET @counter = @counter + 1;
    END
    CLOSE legalPersonCursor;
    DEALLOCATE legalPersonCursor;

    DECLARE accountCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Accounts A
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_account] MA ON A.ReplicationCode=MA.[Dg_accountId]
			WHERE MA.ModifiedOn IS NULL OR A.ModifiedOn!=MA.ModifiedOn
    OPEN accountCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM accountCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация лицевого счета с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateAccount] @id;
          SET @counter = @counter + 1;
    END
    CLOSE accountCursor;
    DEALLOCATE accountCursor;
    
    DECLARE operationTypeCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OperationTypes OT
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_operationtype] MA ON OT.ReplicationCode=MA.[Dg_operationtypeId]
			WHERE MA.ModifiedOn IS NULL OR OT.ModifiedOn!=MA.ModifiedOn
    OPEN operationTypeCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM operationTypeCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация типа операции по лицевому счету с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOperationType] @id;
          SET @counter = @counter + 1;
    END
    CLOSE operationTypeCursor;
    DEALLOCATE operationTypeCursor;
    
    DECLARE accountDetailCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.AccountDetails AD
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_accountdetail] MA ON AD.ReplicationCode=MA.[Dg_accountdetailId]
			WHERE MA.ModifiedOn IS NULL OR AD.ModifiedOn!=MA.ModifiedOn
    OPEN accountDetailCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM accountDetailCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация операции по лицевому счету с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateAccountDetail] @id;
          SET @counter = @counter + 1;
    END
    CLOSE accountDetailCursor;
    DEALLOCATE accountDetailCursor;

    DECLARE dealCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Deals D
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[OpportunityBase] MO ON D.ReplicationCode=MO.[OpportunityId]
			WHERE MO.ModifiedOn IS NULL OR D.ModifiedOn!=MO.ModifiedOn
    OPEN dealCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM dealCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация сделки с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateDeal] @id;
          SET @counter = @counter + 1;
    END
    CLOSE dealCursor;
    DEALLOCATE dealCursor;

    DECLARE limitCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Limits L
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_limitBase] ML ON L.ReplicationCode=ML.[Dg_limitId]
			WHERE ML.ModifiedOn IS NULL OR L.ModifiedOn!=ML.ModifiedOn
    OPEN limitCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM limitCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация лимита с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateLimit] @id;
          SET @counter = @counter + 1;
    END
    CLOSE limitCursor;
    DEALLOCATE limitCursor;

    DECLARE orderCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Orders O
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_orderBase] MO ON O.ReplicationCode=MO.[Dg_orderId]
			WHERE MO.ModifiedOn IS NULL OR O.ModifiedOn!=MO.ModifiedOn
    OPEN orderCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM orderCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация заказа с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOrder] @id;
          SET @counter = @counter + 1;
    END
    CLOSE orderCursor;
    DEALLOCATE orderCursor;

    DECLARE orderPositionCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.OrderPositions OP
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_orderpositionBase] MOP ON OP.ReplicationCode=MOP.[Dg_orderpositionId]
			WHERE MOP.ModifiedOn IS NULL OR OP.ModifiedOn!=MOP.ModifiedOn
    OPEN orderPositionCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM orderPositionCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация позиции заказа с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateOrderPosition] @id;
          SET @counter = @counter + 1;
    END
    CLOSE orderPositionCursor;
    DEALLOCATE orderPositionCursor;

    DECLARE bargainCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
			SELECT Id from Billing.Bargains B
			LEFT JOIN [DoubleGis_MSCRM].[dbo].[Dg_bargain] MB ON B.ReplicationCode=MB.[Dg_bargainId]
			WHERE MB.ModifiedOn IS NULL OR B.ModifiedOn!=MB.ModifiedOn
    OPEN bargainCursor;
    WHILE 1 = 1
    BEGIN
          FETCH NEXT FROM bargainCursor INTO @id;
          IF @@FETCH_STATUS <> 0 BREAK;
          PRINT N'Репликация договора с идентификатором '+ CONVERT(varchar(25), @id)
          EXECUTE [Billing].[ReplicateBargain] @id;
          SET @counter = @counter + 1;
    END
    CLOSE bargainCursor;
    DEALLOCATE bargainCursor;

    PRINT @counter;

--    COMMIT TRANSACTION TxReplicateEverything
    SET NOCOUNT OFF

