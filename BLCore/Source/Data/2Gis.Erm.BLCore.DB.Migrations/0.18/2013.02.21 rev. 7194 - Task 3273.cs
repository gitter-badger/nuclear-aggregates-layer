using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Databases.Erm.Migrations.Impl
{
    [Migration(7194, "Миграции к задаче 3273")]
    public sealed class Migration7194 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // препарируем firmaddress (создаём addresscode, выносим reference point)
            AddAddressCodeColumn(context);
            AddReferencePointColumn(context);
            AddReferencePointColumn(context);
            AlterImportCardsFromXml(context);

            // допиливаем postintegrationactivities (создаём Id, SyncFirmAddresses, alter на SP)
            AddIdColumn(context);
            AddSyncFirmAddressesColumn(context);
            AddFirmsRelations(context);
            AlterExportOrdersAfterImportCardsSp(context);
            AlterReplicateObjectsAfterImportCardsSp(context);
        }

        private static void AlterImportCardsFromXml(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ImportCardsFromXml", ErmSchemas.Integration];
            sp.TextBody = @"
BEGIN

SET XACT_ABORT ON;

IF Object_Id('tempdb..#XmlCardTbl') IS NOT NULL DROP TABLE #XmlCardTbl
IF Object_Id('tempdb..#XmlContactsTbl') IS NOT NULL DROP TABLE #XmlContactsTbl
IF Object_Id('tempdb..#SchedulesTbl') IS NOT NULL DROP TABLE #SchedulesTbl

CREATE TABLE #XmlContactsTbl
	(InsertOrder INT IDENTITY(1, 1) NOT NULL,
	CardCode BIGINT NOT NULL,
	ContactName NVARCHAR(250) NOT NULL,
	Contact NVARCHAR(512),
	ZoneCode NVARCHAR(512),
	FormatCode NVARCHAR(1024),
	CanReceiveFax BIT,
	IsContactInactive BIT)

CREATE TABLE #XmlCardTbl
	(CardCode BIGINT NOT NULL,
	FirmCode BIGINT NOT NULL,
	BranchCode INT NOT NULL,
	CardType nvarchar(3) NOT NULL,
	BuildingCode BIGINT,
	AddressCode BIGINT,
	Address NVARCHAR(max),
	ReferencePoint NVARCHAR(1024),
	IsCardHiddenOrArchived BIT,
	WorkingTime NVARCHAR(512),
	PaymentMethods NVARCHAR(512),
	IsHidden BIT,
	IsArchived BIT,
	IsDeleted BIT,
	FirmId INT,
	AddressId INT,
	OrganizationUnitId INT,
	IsLocal BIT)

CREATE TABLE #SchedulesTbl (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, Name NVARCHAR(2048), DayLabel NVARCHAR(3), DayFrom Time, DayTo Time, BreakFrom NVARCHAR(1024), BreakTo NVARCHAR(1024), SortingPosition INT NOT NULL, WorkingTime NVARCHAR(512))

                 
BEGIN TRY
        
       IF (@Doc IS NULL)
       BEGIN
             SELECT NULL
             RETURN
       END

       DECLARE @ContactTypes table(
             ID int NOT NULL,
             Name nvarchar(10));

       INSERT INTO @ContactTypes (ID, Name) VALUES
	   (0, 'None'),
       (1, 'Phone'),
       (2, 'Fax'),
       (3, 'Email'),
       (4, 'Web'),
       (5, 'Icq'),
       (6, 'Skype'),
       (7, 'Other')

       DECLARE @idoc int

    BEGIN TRAN

       EXEC sp_xml_preparedocument @idoc OUTPUT, @Doc

       INSERT INTO #XmlCardTbl 
       SELECT 
             CardCode,
             FirmCode,
             BranchCode,
             CardType,
             BuildingCode,
			 AddressCode,
             Address,
             ISNULL(PromotionalReferencePoint, ReferencePoint),
             ISNULL(IsCardHidden, 0) | ISNULL(IsCardArchived, 0) | ISNULL(IsCardDeleted, 0),
             NULL,
             NULL,
             ISNULL(IsCardHidden, 0),
             ISNULL(IsCardArchived, 0),
             ISNULL(IsCardDeleted, 0),
             NULL,
             NULL,
             NULL,
			 ISNULL(IsLocal, 1)
       FROM OPENXML (@idoc, '/Root/Card',1)
            WITH (
                    CardCode BIGINT '@Code',
                    FirmCode BIGINT '@FirmCode', 
                    BranchCode BIGINT '@BranchCode',
                    CardType nvarchar(3) '@Type',
                    IsCardHidden BIT '@IsHidden',
                    IsCardArchived  BIT '@IsArchived',
                    IsCardDeleted  BIT '@IsDeleted',
                    BuildingCode BIGINT './Address/@BuildingCode',
					AddressCode BIGINT './Address/@AddressCode',
                    Address nvarchar(max) './Address/@Text',
                    PromotionalReferencePoint nvarchar(max) './Address/@PromotionalReferencePoint',
                    ReferencePoint nvarchar(max) './Address/@ReferencePoint',
					IsLocal BIT '@IsLocal')

       INSERT INTO #XmlContactsTbl SELECT
             Code,
             ContactName,    
             Value,
             ZoneCode,
             FormatCode,
             CanReceiveFax,
             ISNULL(IsHidden, 0) | ISNULL(IsArchived, 0) | ISNULL(NotPublish, 0)
       FROM OPENXML (@idoc, '/Root/Card/Contacts/*',1)
            WITH (ContactName  nvarchar(250) '@mp:localname',
                    ZoneCode nvarchar(max),
                    FormatCode nvarchar(max),
                    CanReceiveFax Bit,
                    Value nvarchar(max),
                    Code BIGINT '../../@Code', 
                    IsPromotional BIT,
                    IsCommonContact BIT,
                    IsHidden BIT,
                    IsArchived BIT,
                    NotPublish BIT)

       DECLARE @CategoriesDgppIdsTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, CategoryDgppId INT NOT NULL, AddressId INT, SortingPosition INT NOT NULL, CategoryId INT, IsPrimary Bit)
       INSERT INTO @CategoriesDgppIdsTbl SELECT
             CardCode,
             RubricCode,
             NULL,
             0,
             NULL,
			 ISNULL(IsPrimary, 0)
       FROM OPENXML (@idoc, '/Root/Card/Rubrics/Rubric',1)
            WITH (
                    CardCode BIGINT '../../@Code', 
                     RubricCode INT '@Code',
					 IsPrimary BIT '@IsPrimary')

       INSERT INTO #SchedulesTbl SELECT
             CardCode,
             Name,
             DayLabel,
             DayFrom,
             DayTo,
             BreakFrom,
             BreakTo,
             0,
             NULL
       FROM OPENXML (@idoc, '/Root/Card/Schedules/Schedule/*',1)
            WITH (
                    CardCode BIGINT '../../../@Code', 
                    Name NVARCHAR(max) '../@Name',
                    DayLabel NVARCHAR(3) '@Label',
                    DayFrom Time '@From', 
                    DayTo Time '@To',
                    BreakFrom Time './*/@From',
                    BreakTo Time './*/@To')

       DECLARE @PaymentTypesTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, ItemCode INT NOT NULL, Name NVARCHAR(max), SortingPosition INT NOT NULL)
       INSERT INTO  @PaymentTypesTbl SELECT 
             CardCode,
             ItemCode,
             Integration.ReferenceItems.Name,
             0
       FROM OPENXML (@idoc, '/Root/Card/Fields/ReferenceListField[@Code=''PaymentMethod'']/Items/*',1)
             WITH (
             CardCode BIGINT '../../../../@Code',
             ItemCode INT '@Code') as XmlValues left join Integration.ReferenceItems On XmlValues.ItemCode = Integration.ReferenceItems.Code AND Integration.ReferenceItems.IsDeleted = 0 
              AND Integration.ReferenceItems.ReferenceId IN (SELECT Id FROM Integration.Reference WHERE CodeName = 'PaymentMethod')
           
       EXEC sp_xml_removedocument @idoc

             -- Если заполнять сразу в @XmlContactDtos из Xml нарушится порядок вставки
       DECLARE @XmlContactDtos TABLE (InsertOrder INT NOT NULL, CardCode BIGINT NOT NULL, ContactName NVARCHAR(250) NOT NULL, ContactTypeId INT NOT NULL, Contact NVARCHAR(max), ZoneCode NVARCHAR(max),FormatCode NVARCHAR(max), CanReceiveFax BIT, ZoneName NVARCHAR(max), FormatName NVARCHAR(max), IsContactInactive BIT, SortingPosition INT, AddressId INT)

       INSERT INTO  @XmlContactDtos 
       SELECT
             InsertOrder, 
             CardCode, 
             ContactName,
             ISNULL(types.ID, 7),
             Contact,
             ZoneCode,
             FormatCode,
             CanReceiveFax,
             zones.Name,
             formats.Name,
             IsContactInactive,
             0,
             NULL
       
       FROM #XmlContactsTbl 
                            left join @ContactTypes as types on ContactName = types.Name 
                            left join [Integration].CityPhoneZone as zones on ZoneCode = zones.Code  
                            left join [Integration].ReferenceItems as formats on FormatCode = formats.Code
                           
       UPDATE @XmlContactDtos set ContactTypeId = 2 Where ContactTypeId = 1 AND CanReceiveFax = 'True' -- телефон с функцией факса - это факс в понимании ERM
       UPDATE @XmlContactDtos set Contact = [Shared].FormatPhoneAndFax(Contact, FormatName, ZoneName) Where ContactTypeId = 1 OR ContactTypeId = 2 AND FormatName IS NOT NULL

       DELETE FROM @XmlContactDtos WHERE IsContactInactive = 1

       UPDATE @XmlContactDtos
       SET SortingPosition = CalculatedSortingPosition
       FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @XmlContactDtos) CalculatedCards
       INNER JOIN @XmlContactDtos Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

       DECLARE @DepCards Integration.CardsTableType
       INSERT INTO @DepCards
       SELECT * FROM #XmlCardTbl WHERE CardType = 'DEP'

       DECLARE @DepContacts Integration.ContactDTOsTableType
       INSERT INTO @DepContacts
       SELECT * FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)

       EXEC Integration.ImportDepCardsFromXml @XmlCardTbl = @DepCards, @XmlContactDtos = @DepContacts, @ModifiedBy = @ModifiedBy, @EnableReplication = @EnableReplication

	   DECLARE @FirmIds TABLE (Id INT NOT NULL, FirmCode BIGINT NOT NULL)

	   INSERT INTO @FirmIds
       SELECT DISTINCT BusinessDirectory.Firms.Id, CardsTable.FirmCode 
	   FROM #XmlCardTbl as CardsTable
       inner join BusinessDirectory.Firms ON CardsTable.FirmCode = BusinessDirectory.Firms.DgppId 

       DELETE FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)
       DELETE FROM #XmlCardTbl WHERE CardType = 'DEP'
       DELETE FROM @PaymentTypesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
       DELETE FROM #SchedulesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
       DELETE FROM @CategoriesDgppIdsTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)   

       -- Проставляем способы оплаты
       UPDATE @PaymentTypesTbl
             SET SortingPosition = CalculatedSortingPosition
       FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @PaymentTypesTbl) CalculatedCards
             INNER JOIN @PaymentTypesTbl Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

       DECLARE @CurrentCardCode BIGINT
       DECLARE @CurrentPaymentId INT
       DECLARE @CurrentPaymentMethod NVARCHAR(max)

       SET @CurrentCardCode = (SELECT MIN(CardCode) FROM @PaymentTypesTbl)
       WHILE @CurrentCardCode IS NOT NULL
       BEGIN
             SET @CurrentPaymentId = (SELECT MIN(InsertOrder) FROM @PaymentTypesTbl WHERE CardCode = @CurrentCardCode)
             SET @CurrentPaymentMethod = ''
             WHILE @CurrentPaymentId IS NOT NULL
             BEGIN
                    SET @CurrentPaymentMethod = @CurrentPaymentMethod + (SELECT LTRIM(STR(SortingPosition))+'. '+Name+' ' FROM @PaymentTypesTbl WHERE InsertOrder = @CurrentPaymentId)
                    SET @CurrentPaymentId = (SELECT MIN(InsertOrder) FROM @PaymentTypesTbl WHERE CardCode = @CurrentCardCode AND InsertOrder>@CurrentPaymentId)
             END
             UPDATE #XmlCardTbl Set PaymentMethods = @CurrentPaymentMethod WHERE CardCode = @CurrentCardCode
             SET @CurrentCardCode = (SELECT MIN(CardCode) FROM @PaymentTypesTbl WHERE CardCode>@CurrentCardCode)
       END

       -- Проставим WorkTime карточкам
       UPDATE #SchedulesTbl
             SET SortingPosition = CalculatedSortingPosition
       FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM #SchedulesTbl) CalculatedCards
             INNER JOIN #SchedulesTbl Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

       DELETE FROM #SchedulesTbl WHERE SortingPosition > 7 -- Оставляем первое расписание для каждой карточки

       UPDATE #SchedulesTbl SET WorkingTime = Shared.GetWorkingTIme(DayLabel, DayFrom, DayTo, BreakFrom, BreakTo)

       DECLARE @CurrentDayId INT
       DECLARE @CurrentWorkingTime NVARCHAR(max)

       SET @CurrentCardCode = (SELECT MIN(CardCode) FROM #SchedulesTbl)
       WHILE @CurrentCardCode IS NOT NULL
       BEGIN
             SET @CurrentDayId = (SELECT MIN(InsertOrder) FROM #SchedulesTbl WHERE CardCode = @CurrentCardCode)
             SET @CurrentWorkingTime = ''
             WHILE @CurrentDayId IS NOT NULL
             BEGIN
                    SET @CurrentWorkingTime = @CurrentWorkingTime + (SELECT WorkingTime FROM #SchedulesTbl WHERE InsertOrder = @CurrentDayId)
                    SET @CurrentDayId = (SELECT MIN(InsertOrder) FROM #SchedulesTbl WHERE CardCode = @CurrentCardCode AND InsertOrder>@CurrentDayId)
             END
             UPDATE #XmlCardTbl Set WorkingTime = @CurrentWorkingTime WHERE CardCode = @CurrentCardCode
             SET @CurrentCardCode = (SELECT MIN(CardCode) FROM #SchedulesTbl WHERE CardCode>@CurrentCardCode)
       END

       DECLARE @TerritoriesTbl TABLE
       (
       TerritoryId INT NOT NULL,
       OrganizationUnitId INT NOT NULL
       )

       UPDATE #XmlCardTbl
       SET OrganizationUnitId = OrganizationUnits.Id
       FROM #XmlCardTbl Cards
       INNER JOIN Billing.OrganizationUnits OrganizationUnits ON Cards.BranchCode = OrganizationUnits.DgppId

       DECLARE @BranchCodes Shared.OrganizationUnitsTableType 
       INSERT INTO @BranchCodes SELECT DISTINCT OrganizationUnitId  FROM #XmlCardTbl

       INSERT INTO @TerritoriesTbl EXEC Shared.GetTemporaryTerritories @OrganizationUnits = @BranchCodes, @ModifiedBy = @ModifiedBy, @OwnerCode = @OwnerCode, @EnableReplication = @EnableReplication

       DECLARE @FirmsTbl TABLE
       (
       FirmCode BIGINT NULL,
       BranchCode INT NULL,
       FirmId INT,
       TerritoryId INT NOT NULL,
       OrganizationUnitId INT NOT NULL
       )

       INSERT INTO @FirmsTbl
       SELECT DISTINCT CardsTable.FirmCode, CardsTable.BranchCode, BusinessDirectory.Firms.Id, Territories.TerritoryId, CardsTable.OrganizationUnitId FROM #XmlCardTbl as CardsTable 
       left join BusinessDirectory.Firms ON CardsTable.FirmCode = BusinessDirectory.Firms.DgppId 
       inner join @TerritoriesTbl as Territories ON CardsTable.OrganizationUnitId = Territories.OrganizationUnitId 
       
       -- Создаем пустые фирмы, если это необходимо
       IF EXISTS (SELECT * FROM @FirmsTbl WHERE FirmId IS NULL)
       BEGIN
             INSERT BusinessDirectory.Firms(DgppId, Name, TerritoryId, OrganizationUnitId, UsingOtherMedia, ProductType, MarketType, OwnerCode, IsActive, ClosedForAscertainment, ReplicationCode, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
             OUTPUT inserted.Id, inserted.DgppId INTO @FirmIds
             SELECT FirmCode, 'Пустая фирма #' + LTRIM(STR(FirmCode)),TerritoryId, OrganizationUnitId, 0, 0, 0, @OwnerCode, 0, 1, NEWID(), @ModifiedBy, GETUTCDATE(), @ModifiedBy, GETUTCDATE()
             FROM @FirmsTbl WHERE FirmId IS NULL
             
             UPDATE @FirmsTbl 
             SET FirmId = NewValues.Id
             FROM @FirmsTbl CurrentValues inner join @FirmIds NewValues ON CurrentValues.FirmCode = NewValues.FirmCode
       END

       UPDATE #XmlCardTbl
       SET FirmId = Firms.FirmId
       FROM #XmlCardTbl Cards
       INNER JOIN @FirmsTbl Firms ON Cards.FirmCode = Firms.FirmCode

       --Обновляем здания
       INSERT INTO Integration.Buildings(Code, TerritoryId, IsDeleted)
       SELECT DISTINCT CardsTable.BuildingCode, min(Territories.TerritoryId), 0 FROM  #XmlCardTbl as CardsTable 
       inner join @TerritoriesTbl as Territories ON CardsTable.OrganizationUnitId = Territories.OrganizationUnitId WHERE CardsTable.BuildingCode NOT IN (SELECT DISTINCT Code FROM Integration.Buildings) GROUP BY CardsTable.BuildingCode

       --Обновляем адреса
       DECLARE @AddressIds TABLE (Id INT NOT NULL)

       MERGE BusinessDirectory.FirmAddresses AS CurrentValues
       USING #XmlCardTbl AS NewValues 
       ON (CurrentValues.DgppId = NewValues.CardCode) 
       WHEN MATCHED THEN 
       UPDATE SET 
             CurrentValues.Address = NewValues.Address,
			 CurrentValues.ReferencePoint = NewValues.ReferencePoint,
             CurrentValues.ClosedForAscertainment = NewValues.IsHidden,
			 CurrentValues.IsLocatedOnTheMap = (case when (NewValues.BuildingCode is not null and NewValues.IsLocal = 1) then 1 else 0 end),
             CurrentValues.IsActive = ~NewValues.IsArchived,
             CurrentValues.IsDeleted = NewValues.IsDeleted,
             CurrentValues.PaymentMethods = NewValues.PaymentMethods,
             CurrentValues.WorkingTime = NewValues.WorkingTime,
             CurrentValues.BuildingCode = NewValues.BuildingCode,
			 CurrentValues.AddressCode = NewValues.AddressCode,
             CurrentValues.OwnerCode = @OwnerCode,
             CurrentValues.ModifiedBy = @ModifiedBy,
             CurrentValues.ModifiedOn = GETUTCDATE()
       WHEN NOT MATCHED BY TARGET THEN 
       INSERT (DgppId, Address, ReferencePoint, ClosedForAscertainment, IsActive, IsDeleted, PaymentMethods, WorkingTime, BuildingCode, AddressCode, CreatedBy, CreatedOn, ReplicationCode, FirmId, OwnerCode, ModifiedBy, ModifiedOn, IsLocatedOnTheMap) 
       VALUES (NewValues.CardCode, NewValues.Address, NewValues.ReferencePoint, NewValues.IsHidden, ~NewValues.IsArchived, NewValues.IsDeleted, NewValues.PaymentMethods, 
       NewValues.WorkingTime, NewValues.BuildingCode, NewValues.AddressCode, @ModifiedBy, GETUTCDATE(), NEWID(), NewValues.FirmId, @OwnerCode,  @ModifiedBy, GETUTCDATE(), (case when (NewValues.BuildingCode is not null and NewValues.IsLocal = 1) then 1 else 0 end))
       OUTPUT inserted.Id INTO @AddressIds;

       --Проставляем адреса карточкам, контактам и рубрикам
       UPDATE #XmlCardTbl
       SET 
       AddressId = Addresses.Id
       FROM #XmlCardTbl Cards INNER JOIN BusinessDirectory.FirmAddresses Addresses ON Cards.CardCode = Addresses.DgppId 

       UPDATE @XmlContactDtos
       SET 
       AddressId = Addresses.AddressId
       FROM @XmlContactDtos Contacts INNER JOIN #XmlCardTbl Addresses ON Contacts.CardCode = Addresses.CardCode

       UPDATE @CategoriesDgppIdsTbl
             SET 
                    SortingPosition = CalculatedSortingPosition,
                    AddressId = Addresses.Id,
                    CategoryId = Categories.Id
       FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @CategoriesDgppIdsTbl) CalculatedCards
       INNER JOIN @CategoriesDgppIdsTbl Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder 
       INNER JOIN BusinessDirectory.FirmAddresses Addresses ON Cards.CardCode = Addresses.DgppId 
       INNER JOIN BusinessDirectory.Categories Categories ON Cards.CategoryDgppId = Categories.DgppId

       --Обновляем рубрики
       MERGE BusinessDirectory.CategoryFirmAddresses AS CurrentValues
       USING @CategoriesDgppIdsTbl AS NewValues 
       ON CurrentValues.FirmAddressId = NewValues.AddressId AND CurrentValues.CategoryId = NewValues.CategoryId
       WHEN MATCHED THEN 
       UPDATE SET CurrentValues.IsActive = 1, CurrentValues.IsDeleted = 0, CurrentValues.SortingPosition = NewValues.SortingPosition, CurrentValues.IsPrimary = NewValues.IsPrimary, CurrentValues.ModifiedBy = @ModifiedBy, CurrentValues.ModifiedOn = GETUTCDATE()
	   WHEN NOT MATCHED BY TARGET THEN 
       INSERT (CategoryId, FirmAddressId, IsActive, IsDeleted, CreatedBy, CreatedOn, OwnerCode, SortingPosition, IsPrimary, ModifiedBy, ModifiedOn) 
       VALUES (NewValues.CategoryId, NewValues.AddressId, 1,0,@ModifiedBy, GETUTCDATE(), @OwnerCode, NewValues.SortingPosition, NewValues.IsPrimary, @ModifiedBy, GETUTCDATE());

       UPDATE BusinessDirectory.CategoryFirmAddresses 
       SET IsDeleted = 1, IsActive = 0, ModifiedBy = @ModifiedBy, ModifiedOn = GETUTCDATE()
       WHERE Id IN
       (
             SELECT Id FROM BusinessDirectory.CategoryFirmAddresses AS CurrentValues left join @CategoriesDgppIdsTbl AS NewValues 
             ON CurrentValues.FirmAddressId = NewValues.AddressId AND CurrentValues.CategoryId = NewValues.CategoryId WHERE CurrentValues.FirmAddressId IN (SELECT AddressId FROM #XmlCardTbl) AND NewValues.CardCode IS NULL
       )

       --Обновляем контакты
       MERGE BusinessDirectory.FirmContacts AS CurrentValues
       USING @XmlContactDtos AS NewValues
       ON CurrentValues.FirmAddressId = NewValues.AddressId AND CurrentValues.SortingPosition = NewValues.SortingPosition
       WHEN MATCHED THEN 
             UPDATE SET 
                    CurrentValues.ContactType = NewValues.ContactTypeId,
                    CurrentValues.Contact = NewValues.Contact,
                    CurrentValues.ModifiedBy = @ModifiedBy,
                    CurrentValues.ModifiedOn = GETUTCDATE()
       WHEN NOT MATCHED BY TARGET THEN 
             INSERT (FirmAddressId, Contact, ContactType, CreatedBy, CreatedOn, SortingPosition, ModifiedBy, ModifiedOn)
             VALUES (NewValues.AddressId, NewValues.Contact, NewValues.ContactTypeId, @ModifiedBy, GETUTCDATE(), NewValues.SortingPosition, @ModifiedBy, GETUTCDATE());

       DELETE FROM BusinessDirectory.FirmContacts WHERE Id in 
    (
        SELECT currentContacts.Id FROM BusinessDirectory.FirmContacts as CurrentContacts left join @XmlContactDtos as NewValues ON  NewValues.AddressId = CurrentContacts.FirmAddressId AND NewValues.SortingPosition = CurrentContacts.SortingPosition
        WHERE CurrentContacts.FirmAddressId in (SELECT DISTINCT AddressId FROM #XmlCardTbl) AND NewValues.CardCode is NULL
    )


       INSERT INTO Integration.FirmsForPostIntegrationActivities (FirmId, CreatedOn) SELECT Id, GETUTCDATE() FROM @FirmIds
    COMMIT TRAN
END TRY
BEGIN CATCH
       IF (XACT_STATE() != 0)
             ROLLBACK TRAN

       DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
       SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
       RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH

IF Object_Id('tempdb..#XmlCardTbl') IS NOT NULL DROP TABLE #XmlCardTbl
IF Object_Id('tempdb..#XmlContactsTbl') IS NOT NULL DROP TABLE #XmlContactsTbl
IF Object_Id('tempdb..#SchedulesTbl') IS NOT NULL DROP TABLE #SchedulesTbl
END";

            sp.Alter();
        }

        private static void AddFirmsRelations(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmsForPostIntegrationActivities", ErmSchemas.Integration];

            var foreignKey = table.ForeignKeys["FK_FirmsForPostIntegrationActivities_Firms"];
            if (foreignKey != null)
            {
                return;
            }
            
            foreignKey = new ForeignKey(table, "FK_FirmsForPostIntegrationActivities_Firms");
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "FirmId", "Id"));
            foreignKey.ReferencedTable = "Firms";
            foreignKey.ReferencedTableSchema = ErmSchemas.BusinessDirectory;
            foreignKey.Create();
        }

        private static void AddIdColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmsForPostIntegrationActivities", ErmSchemas.Integration];

            var column = table.Columns["Id"];
            if (column != null)
            {
                return;
            }

            column = new Column(table, "Id", DataType.Int) { Identity = true, IdentitySeed = 1, IdentityIncrement = 1 };
            column.Create();

            // TODO {m.pashuk, 27.02.2013}: Ради вселенского равновесия и соблюдения феншуя можно вставить колонку не в конец, а в начало таблицы (см класс Migration7015).
            table.CreatePrimaryKey();
        }

        private static void AddSyncFirmAddressesColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmsForPostIntegrationActivities", ErmSchemas.Integration];

            var column = table.Columns["SyncFirmAddresses"];
            if (column != null)
            {
                return;
            }

            column = new Column(table, "SyncFirmAddresses", DataType.Bit) { Nullable = false };
            column.AddDefaultConstraint("DF_FirmsForPostIntegrationActivities_SyncFirmAddresses").Text = "0";
            column.Create();
        }

        private static void AlterReplicateObjectsAfterImportCardsSp(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ReplicateObjectsAfterImportCards", ErmSchemas.Integration];
            sp.TextBody = @"
BEGIN
SET XACT_ABORT ON;

BEGIN TRY;

--Репликация объектов
BEGIN TRAN

DECLARE @FirmIds Shared.Int32IdsTableType
INSERT INTO @FirmIds SELECT DISTINCT FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ReplicateObjects = 0

DECLARE @AddressIds Shared.Int32IdsTableType
INSERT INTO @AddressIds SELECT Id FROM BusinessDirectory.FirmAddresses WHERE FirmId IN (SELECT Id FROM @FirmIds)

EXEC BusinessDirectory.ReplicateFirms @FirmIds
EXEC BusinessDirectory.ReplicateFirmAddresses @AddressIds

UPDATE Integration.FirmsForPostIntegrationActivities SET ReplicateObjects = 1
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1 AND SyncFirmAddresses = 1

COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END";
            sp.Alter();
        }

        private static void AlterExportOrdersAfterImportCardsSp(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ExportOrdersAfterImportCards", ErmSchemas.Integration];
            sp.TextBody = @"
BEGIN
SET XACT_ABORT ON;

BEGIN TRY;

--Выгружаем заказы, связанные с этими фирмами
BEGIN TRAN

DECLARE @ArchiveWorkflowStepId INT = 6
DECLARE @FakeField INT
UPDATE Billing.Orders 
SET @FakeField = 1
WHERE
WorkflowStepId != @ArchiveWorkflowStepId
AND
FirmId IN (SELECT FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 0)

UPDATE Integration.FirmsForPostIntegrationActivities SET ExportOrders = 1
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1 AND SyncFirmAddresses = 1

COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END";
            sp.Alter();
        }

        private static void AddAddressCodeColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmAddresses", ErmSchemas.BusinessDirectory];

            var column = table.Columns["AddressCode"];
            if (column != null)
            {
                return;
            }

            column = new Column(table, "AddressCode", DataType.BigInt) { Nullable = true };
            // TODO {m.pashuk, 27.02.2013}: Эту (и следующую) колонку тоже можно вставить не в конец, 
            // а в середину таблицы одним вызовом  EntityCopyHelper.CopyAndInsertColumns (см класс Migration4935).
            column.Create();
        }

        private static void AddReferencePointColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmAddresses", ErmSchemas.BusinessDirectory];

            var column = table.Columns["ReferencePoint"];
            if (column != null)
            {
                return;
            }

            // 200 это эмпирическиое число, т.к. в IR нет вообще никаких ограничений, там xml хранится как blob и размеры не важны
            column = new Column(table, "ReferencePoint", DataType.NVarChar(200)) { Nullable = true };
            column.Create();

            FillReferencePointColumn(context);
        }

        private static void FillReferencePointColumn(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            UPDATE BusinessDirectory.FirmAddresses
            SET

            Address =
            (CASE 
            WHEN CHARINDEX(' ' + nchar(0x2014) + ' ', Address) = 0 THEN Address
            ELSE SUBSTRING(Address, 0, CHARINDEX(' ' + nchar(0x2014) + ' ', Address))
            END),

            ReferencePoint=
            (CASE 
            WHEN CHARINDEX(' ' + nchar(0x2014) + ' ', Address) = 0 THEN NULL
            ELSE SUBSTRING(Address, CHARINDEX(' ' + nchar(0x2014) + ' ', Address) + 3, 200)
            END)
            ");
        }
    }
}