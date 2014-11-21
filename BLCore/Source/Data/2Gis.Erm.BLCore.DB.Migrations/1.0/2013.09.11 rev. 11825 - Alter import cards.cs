using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11825, "Alter на ImportCardsFromXml")]
    public class Migration11825 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropImportDepCardsFromXml(context);
            DropAndCreateContactDTOsTableType(context);
            CreateImportDepCardsFromXml(context);
            AlterImportCardsFromXml(context);
            AlterUpdateBuildings(context);
            AlterImportFirmFromXml(context);
        }

        private void DropImportDepCardsFromXml(IMigrationContext context)
        {
            const string query = @"
DROP Procedure [Integration].[ImportDepCardsFromXml]
GO";
            context.Database.ExecuteNonQuery(query);
        }

        private void DropAndCreateContactDTOsTableType(IMigrationContext context)
        {
            const string query = @"
DROP TYPE [Integration].[ContactDTOsTableType]
GO

CREATE TYPE [Integration].[ContactDTOsTableType] AS TABLE(
	[InsertOrder] [int] NOT NULL,
	[CardCode] [bigint] NOT NULL,
	[ContactName] [nvarchar](250) NOT NULL,
	[ContactTypeId] [int] NOT NULL,
	[Contact] [nvarchar](max) NULL,
	[ZoneCode] [nvarchar](max) NULL,
	[FormatCode] [nvarchar](max) NULL,
	[CanReceiveFax] [bit] NULL,
	[ZoneName] [nvarchar](max) NULL,
	[FormatName] [nvarchar](max) NULL,
	[IsContactInactive] [bit] NULL,
	[SortingPosition] [int] NULL,
	[AddressId] [bigint] NULL, 
	[PregeneratedId] [bigint] NULL 
)
GO";

            context.Database.ExecuteNonQuery(query);
        }

        private void CreateImportDepCardsFromXml(IMigrationContext context)
        {
            const string query = @"
-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   10.09.2013, y.baranihin: использование сгенерированних идентификаторов
CREATE PROCEDURE [Integration].[ImportDepCardsFromXml]
    @XmlCardTbl Integration.CardsTableType READONLY,
	@XmlContactDtos  Integration.ContactDTOsTableType READONLY,
	@ModifiedBy bigint = NULL
AS 
BEGIN

SET XACT_ABORT ON;

BEGIN TRY 

BEGIN TRAN

MERGE Integration.DepCards AS CurrentValues
USING @XmlCardTbl AS NewValues 
ON (CurrentValues.Code = NewValues.CardCode) 
WHEN MATCHED THEN 
UPDATE SET CurrentValues.IsHiddenOrArchived = NewValues.IsCardHiddenOrArchived 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Code, IsHiddenOrArchived) 
VALUES (NewValues.CardCode, NewValues.IsCardHiddenOrArchived);

MERGE BusinessDirectory.FirmContacts AS CurrentValues
USING @XmlContactDtos AS NewValues
ON CurrentValues.CardId = NewValues.CardCode AND CurrentValues.SortingPosition = NewValues.SortingPosition
WHEN MATCHED THEN 
UPDATE SET CurrentValues.ContactType = NewValues.ContactTypeId,
CurrentValues.Contact = NewValues.Contact,
CurrentValues.ModifiedBy = @ModifiedBy,
CurrentValues.ModifiedOn = GETUTCDATE()
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, CardId, Contact, ContactType, CreatedBy, CreatedOn, SortingPosition)
VALUES (NewValues.PregeneratedId, NewValues.CardCode, NewValues.Contact, NewValues.ContactTypeId, @ModifiedBy, GETUTCDATE(), NewValues.SortingPosition);

DELETE FROM BusinessDirectory.FirmContacts WHERE Id in 
(
SELECT currentContacts.Id FROM BusinessDirectory.FirmContacts as currentContacts left join @XmlContactDtos as NewValues ON  NewValues.CardCode = currentContacts.CardId AND NewValues.SortingPosition = currentContacts.SortingPosition
WHERE currentContacts.CardId in (SELECT DISTINCT CardCode FROM @XmlCardTbl) AND NewValues.CardCode is NULL
)

COMMIT TRAN

END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END

GO";
            context.Database.ExecuteNonQuery(query);
        }

        private void AlterImportCardsFromXml(IMigrationContext context)
        {
            const string query = @"-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   5.06.2013, a.rechkalov: добавил условие, чтобы в Integation.Builings не пытался вставиться NULL
--   24.06.2013, a.rechkalov: замена int -> bigint
--   5.07.2013, a.rechkalov: слияние с версией из 0.18, из ветки пришло изменение: возврат идентификаторов фирм из хранимки вместо записи их в таблицу
--   9.07.2013, v.sinitsyn: поправлен вызов STR(FirmCode): для больших значений возвращал '*********************'
--  27.08.2013, y.baranihin: удалено чтение неиспользуемых полей
--  10.09.2013, y.baranihin: использование сгенерированних идентификаторов
ALTER PROCEDURE [Integration].[ImportCardsFromXml]
    @Doc xml = NULL, 
    @ModifiedBy bigint = NULL,
    @OwnerCode bigint = NULL,
	@RegionalTerritoryLocalName nvarchar(255),
	@PregenaratedIds Shared.Int64IdsTableType READONLY
AS 
BEGIN
	SET XACT_ABORT ON;

	IF Object_Id('tempdb..#XmlCardTbl') IS NOT NULL DROP TABLE #XmlCardTbl
	IF Object_Id('tempdb..#XmlContactsTbl') IS NOT NULL DROP TABLE #XmlContactsTbl
	IF Object_Id('tempdb..#SchedulesTbl') IS NOT NULL DROP TABLE #SchedulesTbl

	DECLARE @Ids Shared.Int64IdsTableType
	INSERT INTO @Ids SELECT * FROM @PregenaratedIds

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
        FirmId bigint,
        AddressId bigint,
        OrganizationUnitId bigint,
		IsLocal BIT)

	CREATE TABLE #SchedulesTbl (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, DayLabel NVARCHAR(3), DayFrom Time, DayTo Time, BreakFrom NVARCHAR(1024), BreakTo NVARCHAR(1024), SortingPosition INT NOT NULL, WorkingTime NVARCHAR(512))

                 
	BEGIN TRY
        
		   IF (@Doc IS NULL)
		   BEGIN
				 SELECT NULL
				 RETURN
		   END

		   DECLARE @ContactTypes table(
                 ID bigint NOT NULL,
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
						IsHidden BIT,
						IsArchived BIT,
						NotPublish BIT)

           DECLARE @CategoriesDgppIdsTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, CategoryDgppId INT NOT NULL, AddressId bigint, SortingPosition INT NOT NULL, CategoryId bigint, IsPrimary Bit, PregeneratedId bigint)
		   INSERT INTO @CategoriesDgppIdsTbl SELECT
				 CardCode,
				 RubricCode,
				 NULL,
				 0,
				 NULL,
				 ISNULL(IsPrimary, 0),
				 NULL
		   FROM OPENXML (@idoc, '/Root/Card/Rubrics/Rubric',1)
				WITH (
						CardCode BIGINT '../../@Code', 
						 RubricCode INT '@Code',
						 IsPrimary BIT '@IsPrimary')

		   INSERT INTO #SchedulesTbl SELECT
				 CardCode,
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
           DECLARE @XmlContactDtos Integration.ContactDTOsTableType

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
				 NULL,
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

		   -- Проставим идентификаторы для контактов фирмы
		   DECLARE @CurrentContactId int -- ориентируемся на InsertOrder
		   DECLARE @CurrentPregeneratedId bigint
		   SET @CurrentContactId = (SELECT min(InsertOrder) FROM @XmlContactDtos)
		   WHILE @CurrentContactId IS NOT NULL
		   BEGIN
			SET @CurrentPregeneratedId = (SELECT TOP 1 Id FROM @Ids)
			if @CurrentPregeneratedId IS NULL 
			 begin
			   RAISERROR('Сгенерировано недостаточно идентификаторов', 16, 2)
			end
			DELETE FROM @Ids WHERE Id = @CurrentPregeneratedId
			UPDATE @XmlContactDtos SET PregeneratedId = @CurrentPregeneratedId WHERE InsertOrder = @CurrentContactId
			SET @CurrentContactId = (SELECT min(InsertOrder) FROM @XmlContactDtos WHERE InsertOrder > @CurrentContactId)
		   END

		   DECLARE @DepCards Integration.CardsTableType
		   INSERT INTO @DepCards
		   SELECT * FROM #XmlCardTbl WHERE CardType = 'DEP'

		   DECLARE @DepContacts Integration.ContactDTOsTableType
		   INSERT INTO @DepContacts
		   SELECT * FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)

		   EXEC Integration.ImportDepCardsFromXml @XmlCardTbl = @DepCards, @XmlContactDtos = @DepContacts, @ModifiedBy = @ModifiedBy

           DECLARE @FirmIds TABLE (Id bigint NOT NULL)

		   INSERT INTO @FirmIds
		   SELECT DISTINCT BusinessDirectory.Firms.Id
		   FROM #XmlCardTbl as CardsTable
		   inner join BusinessDirectory.Firms ON CardsTable.FirmCode = BusinessDirectory.Firms.Id 

		   DELETE FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)
		   DELETE FROM #XmlCardTbl WHERE CardType = 'DEP'
		   DELETE FROM @PaymentTypesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
		   DELETE FROM #SchedulesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
		   DELETE FROM @CategoriesDgppIdsTbl WHERE CardCode in (SELECT CardCode FROM @DepCards) 
		   
		   -- Проставим идентификаторы для связок рубрик с адресами
		   DECLARE @CurrentCategoryId int -- ориентируемся на InsertOrder
		   SET @CurrentCategoryId = (SELECT min(InsertOrder) FROM @CategoriesDgppIdsTbl)
		   WHILE @CurrentCategoryId IS NOT NULL
		   BEGIN
			SET @CurrentPregeneratedId = (SELECT TOP 1 Id FROM @Ids)
			if @CurrentPregeneratedId IS NULL 
			 begin
			   RAISERROR('Сгенерировано недостаточно идентификаторов', 16, 2)
			end
			DELETE FROM @Ids WHERE Id = @CurrentPregeneratedId
			UPDATE @CategoriesDgppIdsTbl SET PregeneratedId = @CurrentPregeneratedId WHERE InsertOrder = @CurrentCategoryId
			SET @CurrentCategoryId = (SELECT min(InsertOrder) FROM @CategoriesDgppIdsTbl WHERE InsertOrder > @CurrentCategoryId)
		   END  

		   -- Проставляем способы оплаты
		   UPDATE @PaymentTypesTbl
				 SET SortingPosition = CalculatedSortingPosition
		   FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @PaymentTypesTbl) CalculatedCards
				 INNER JOIN @PaymentTypesTbl Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

		   DECLARE @CurrentCardCode BIGINT
           DECLARE @CurrentPaymentId bigint
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
			TerritoryId bigint NOT NULL,
			OrganizationUnitId bigint NOT NULL
		   )

		   -- перевод DgppId в идентификаторы БД
		   UPDATE #XmlCardTbl
		   SET OrganizationUnitId = OrganizationUnits.Id
		   FROM #XmlCardTbl Cards
		   INNER JOIN Billing.OrganizationUnits OrganizationUnits ON Cards.BranchCode = OrganizationUnits.DgppId

		   -- убеждаемся, что все подразделения определились по DgppId успешно
		   declare @InvalidDgppId int
		   select top 1 @InvalidDgppId = BranchCode from #XmlCardTbl where OrganizationUnitId is null
		   if @InvalidDgppId is not null
		   begin
	   		   declare @MessageOrganizationUnitInvalidDgppId as nvarchar(max)
			   -- закомментированный вариант работает, но только в 2012
			   --set @MessageOrganizationUnitInvalidDgppId = concat('Подразделение с кодом ДГПП ', @InvalidDgppId, ' не найдено в системе')
			   set @MessageOrganizationUnitInvalidDgppId = 'Подразделение с кодом ДГПП ' + CONVERT(nvarchar(32), @InvalidDgppId) + ' не найдено в системе'
			   RAISERROR(@MessageOrganizationUnitInvalidDgppId, 16, 2)
		   end

		   DECLARE @BranchCodes Shared.OrganizationUnitsTableType 
		   INSERT INTO @BranchCodes SELECT DISTINCT OrganizationUnitId  FROM #XmlCardTbl

           INSERT INTO @TerritoriesTbl EXEC Shared.GetTemporaryTerritories @OrganizationUnits = @BranchCodes, @ModifiedBy = @ModifiedBy, @RegionalTerritoryLocalName = @RegionalTerritoryLocalName

		   DECLARE @FirmsTbl TABLE
		   (
		   FirmCode BIGINT NULL,
		   BranchCode INT NULL,
           FirmId bigint,
           TerritoryId bigint NOT NULL,
           OrganizationUnitId bigint NOT NULL
		   )

		   INSERT INTO @FirmsTbl
		   SELECT DISTINCT CardsTable.FirmCode, CardsTable.BranchCode, BusinessDirectory.Firms.Id, Territories.TerritoryId, CardsTable.OrganizationUnitId FROM #XmlCardTbl as CardsTable 
		   left join BusinessDirectory.Firms ON CardsTable.FirmCode = BusinessDirectory.Firms.Id 
		   inner join @TerritoriesTbl as Territories ON CardsTable.OrganizationUnitId = Territories.OrganizationUnitId 
       
		   -- Создаем пустые фирмы, если это необходимо
		   IF EXISTS (SELECT * FROM @FirmsTbl WHERE FirmId IS NULL)
		   BEGIN
				 INSERT BusinessDirectory.Firms(Id, Name, TerritoryId, OrganizationUnitId, UsingOtherMedia, ProductType, MarketType, OwnerCode, IsActive, ClosedForAscertainment, ReplicationCode, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
				 OUTPUT inserted.Id INTO @FirmIds
				 SELECT FirmCode, 'Пустая фирма #' + LTRIM(STR(FirmCode, 20)),TerritoryId, OrganizationUnitId, 0, 0, 0, @OwnerCode, 0, 1, NEWID(), @ModifiedBy, GETUTCDATE(), @ModifiedBy, GETUTCDATE()
				 FROM @FirmsTbl WHERE FirmId IS NULL
             
				 UPDATE @FirmsTbl 
				 SET FirmId = NewValues.Id
				 FROM @FirmsTbl CurrentValues inner join @FirmIds NewValues ON CurrentValues.FirmCode = NewValues.Id
		   END

		   UPDATE #XmlCardTbl
		   SET FirmId = Firms.FirmId
		   FROM #XmlCardTbl Cards
		   INNER JOIN @FirmsTbl Firms ON Cards.FirmCode = Firms.FirmCode

		   --Обновляем здания
		   INSERT INTO Integration.Buildings(Code, TerritoryId, IsDeleted)
           SELECT DISTINCT CardsTable.BuildingCode, min(Territories.TerritoryId), 0 
		   FROM  #XmlCardTbl as CardsTable 
               inner join @TerritoriesTbl as Territories ON CardsTable.OrganizationUnitId = Territories.OrganizationUnitId 
		   WHERE CardsTable.BuildingCode NOT IN (SELECT DISTINCT Code FROM Integration.Buildings) 
		       and CardsTable.BuildingCode is not null
		   GROUP BY CardsTable.BuildingCode

		   --Обновляем адреса
           DECLARE @AddressIds TABLE (Id bigint NOT NULL)

		   MERGE BusinessDirectory.FirmAddresses AS CurrentValues
		   USING #XmlCardTbl AS NewValues 
		   ON (CurrentValues.Id = NewValues.CardCode) 
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
				 CurrentValues.ModifiedBy = @ModifiedBy,
				 CurrentValues.ModifiedOn = GETUTCDATE()
		   WHEN NOT MATCHED BY TARGET THEN 
           INSERT (Id, Address, ReferencePoint, ClosedForAscertainment, IsActive, IsDeleted, PaymentMethods, WorkingTime, BuildingCode, AddressCode, CreatedBy, CreatedOn, ReplicationCode, FirmId, ModifiedBy, ModifiedOn, IsLocatedOnTheMap) 
		   VALUES (NewValues.CardCode, NewValues.Address, NewValues.ReferencePoint, NewValues.IsHidden, ~NewValues.IsArchived, NewValues.IsDeleted, NewValues.PaymentMethods, 
           NewValues.WorkingTime, NewValues.BuildingCode, NewValues.AddressCode, @ModifiedBy, GETUTCDATE(), NEWID(), NewValues.FirmId, @ModifiedBy, GETUTCDATE(), (case when (NewValues.BuildingCode is not null and NewValues.IsLocal = 1) then 1 else 0 end))
		   OUTPUT inserted.Id INTO @AddressIds;

		   --Проставляем адреса карточкам, контактам и рубрикам
		   UPDATE #XmlCardTbl
		   SET 
		   AddressId = Addresses.Id
		   FROM #XmlCardTbl Cards INNER JOIN BusinessDirectory.FirmAddresses Addresses ON Cards.CardCode = Addresses.Id 

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
		   INNER JOIN BusinessDirectory.FirmAddresses Addresses ON Cards.CardCode = Addresses.Id 
		   INNER JOIN BusinessDirectory.Categories Categories ON Cards.CategoryDgppId = Categories.Id

		   --Обновляем рубрики
		   MERGE BusinessDirectory.CategoryFirmAddresses AS CurrentValues
		   USING @CategoriesDgppIdsTbl AS NewValues 
		   ON CurrentValues.FirmAddressId = NewValues.AddressId AND CurrentValues.CategoryId = NewValues.CategoryId
		   WHEN MATCHED THEN 
		   UPDATE SET CurrentValues.IsActive = 1, CurrentValues.IsDeleted = 0, CurrentValues.SortingPosition = NewValues.SortingPosition, CurrentValues.IsPrimary = NewValues.IsPrimary, CurrentValues.ModifiedBy = @ModifiedBy, CurrentValues.ModifiedOn = GETUTCDATE()
		   WHEN NOT MATCHED BY TARGET THEN 
           INSERT (Id, CategoryId, FirmAddressId, IsActive, IsDeleted, CreatedBy, CreatedOn, SortingPosition, IsPrimary, ModifiedBy, ModifiedOn) 
           VALUES (NewValues.PregeneratedId, NewValues.CategoryId, NewValues.AddressId, 1,0,@ModifiedBy, GETUTCDATE(), NewValues.SortingPosition, NewValues.IsPrimary, @ModifiedBy, GETUTCDATE());

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
				 INSERT (Id, FirmAddressId, Contact, ContactType, CreatedBy, CreatedOn, SortingPosition, ModifiedBy, ModifiedOn)
				 VALUES (NewValues.PregeneratedId, NewValues.AddressId, NewValues.Contact, NewValues.ContactTypeId, @ModifiedBy, GETUTCDATE(), NewValues.SortingPosition, @ModifiedBy, GETUTCDATE());

		   DELETE FROM BusinessDirectory.FirmContacts WHERE Id in 
		(
			SELECT currentContacts.Id FROM BusinessDirectory.FirmContacts as CurrentContacts left join @XmlContactDtos as NewValues ON  NewValues.AddressId = CurrentContacts.FirmAddressId AND NewValues.SortingPosition = CurrentContacts.SortingPosition
			WHERE CurrentContacts.FirmAddressId in (SELECT DISTINCT AddressId FROM #XmlCardTbl) AND NewValues.CardCode is NULL
		)


		   select Id from @FirmIds
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
END

GO";
            context.Database.ExecuteNonQuery(query);
        }

        private void AlterUpdateBuildings(IMigrationContext context)
        {
            const string query = @"-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
--   10.09.2013, y.baranihin: dgppid->id
ALTER PROCEDURE [Integration].[UpdateBuildings]
       @buildingsXml [xml]
AS
       
	
SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRAN
	
	DECLARE @docHandle INT

	EXEC sp_xml_preparedocument @docHandle OUTPUT, @buildingsXml

	DECLARE @xmlBuildings TABLE (Code BIGINT NOT NULL, SaleTerritoryCode BIGINT NULL, IsDeleted BIT NOT NULL)
	INSERT INTO @xmlBuildings
	SELECT
	Code,
	SaleTerritoryCode,
	COALESCE(IsDeleted, 0) AS IsDeleted -- xsd default value
	FROM OPENXML(@docHandle, '/buildings/building', 1) WITH (Code BIGINT, SaleTerritoryCode BIGINT, IsDeleted BIT)

	IF (EXISTS(SELECT 1 FROM @xmlBuildings xmlBuildings WHERE SaleTerritoryCode NOT IN (SELECT Id FROM BusinessDirectory.Territories) ))
	BEGIN
		RAISERROR ('Cant find SaleTerritoryCode in BusinessDirectory.Territories table', 16, 2) WITH SETERROR
		RETURN
	END

	DECLARE @buildings TABLE(Code BIGINT NOT NULL, TerritoryId bigint NULL, IsDeleted BIT NOT NULL)
	INSERT INTO @buildings
	SELECT
	xmlBuildings.Code,
	T.Id,
	xmlBuildings.IsDeleted
	FROM @xmlBuildings xmlBuildings
	LEFT JOIN BusinessDirectory.Territories T ON T.Id = xmlBuildings.SaleTerritoryCode

	EXEC sp_xml_removedocument @docHandle
	
		-- делаем update существующих и insert отсутствующих записей
	MERGE Integration.Buildings B
	USING (SELECt * FROM @buildings WHERE TerritoryId IS NOT NULL) buildings
	ON buildings.Code = B.Code 
	WHEN MATCHED THEN
		UPDATE SET B.TerritoryId = buildings.TerritoryId,
					B.IsDeleted = buildings.IsDeleted
	WHEN NOT MATCHED THEN
		INSERT (Code, TerritoryId, IsDeleted) VALUES (buildings.Code, buildings.TerritoryId, buildings.IsDeleted);

	UPDATE B 
	SET B.IsDeleted = buildings.IsDeleted
	FROM Integration.Buildings B
	INNER JOIN 
	@buildings buildings ON B.Code = buildings.Code AND buildings.TerritoryId IS NULL


    -- update firm territory
	DECLARE @FirmsToUpdate TABLE (Id bigint NOT NULL, OrganizationUnitId bigint NOT NULL)
    DECLARE @UpdatedFirms TABLE (Id bigint NOT NULL)

	INSERT INTO @FirmsToUpdate
	SELECT F.Id, F.OrganizationUnitId FROM @buildings buildings
	inner join BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = buildings.Code
	inner join BusinessDirectory.Firms F ON F.Id = FA.FirmId

	-- определяем первый активный адрес для фирмы, чтобы далее обновить у нее территорию 
	DECLARE @FirstActiveAdressForFirm TABLE(SortingPosition int NOT NULL, FirmId bigint NOT NULL)
	INSERT INTO @FirstActiveAdressForFirm
	SELECT min(AFA.SortingPosition), AFA.FirmId FROM @FirmsToUpdate UF
	inner join BusinessDirectory.FirmAddresses AFA ON UF.Id = AFA.FirmId AND AFA.IsActive = 1 AND AFA.IsDeleted = 0 AND AFA.ClosedForAscertainment = 0
	inner join Integration.Buildings B ON AFA.BuildingCode = B.Code AND B.IsDeleted = 0
	inner join BusinessDirectory.Firms F ON F.Id = AFA.FirmId
	inner join BusinessDirectory.Territories T ON B.TerritoryId = T.Id
	WHERE F.OrganizationUnitId = T.OrganizationUnitId
	GROUP BY AFA.FirmId 

	UPDATE F
	SET F.TerritoryId = B.TerritoryId,
        F.ModifiedOn = GETUTCDATE(),
        F.ModifiedBy = 1
	OUTPUT inserted.Id INTO @UpdatedFirms
	FROM BusinessDirectory.Firms F 
	INNER JOIN @FirstActiveAdressForFirm FAA ON FAA.FirmId = F.Id
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FAA.FirmId = FA.FirmId AND FAA.SortingPosition = FA.SortingPosition
	inner join Integration.Buildings B ON FA.BuildingCode = B.Code

	-- Выставим региональную территорию тем фирмам, у которых не нашлось подходящаго адреса
		DECLARE @TempTerritoriesTbl TABLE
		(
			TerritoryId bigint, 
			OrganizationUnitId bigint
		)

	INSERT INTO @TempTerritoriesTbl SELECT 
			Id, OrganizationUnitId 
			FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @FirmsToUpdate WHERE Id NOT IN (SELECT Id FROM @UpdatedFirms))
			AND IsActive = 1 AND Name like '%Региональная территория%'

	UPDATE F
	SET F.TerritoryId = AA.TerritoryId,
        F.ModifiedOn = GETUTCDATE(),
        F.ModifiedBy = 1
	OUTPUT inserted.Id INTO @UpdatedFirms
	FROM BusinessDirectory.Firms F
	INNER JOIN @FirmsToUpdate dto ON F.Id = dto.Id
	INNER JOIN @TempTerritoriesTbl AA ON AA.OrganizationUnitId = F.OrganizationUnitId
	WHERE F.Id NOT IN (SELECT Id FROM @UpdatedFirms)

    -- update client territory
    DECLARE @ClientIdTable TABLE (Id bigint NOT NULL)

	UPDATE C
	SET C.TerritoryId = buildings.TerritoryId
	OUTPUT inserted.Id INTO @ClientIdTable
	FROM @buildings buildings
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = buildings.Code
	INNER JOIN BusinessDirectory.Firms F ON F.Id = FA.FirmId
	INNER JOIN Billing.Clients C ON C.Id = F.ClientId
	INNER JOIN BusinessDirectory.Territories T ON T.Id = C.TerritoryId
	WHERE T.IsActive = 0 AND (C.MainFirmId IS NULL OR C.MainFirmId = F.Id)

	-- replicate clients
	DECLARE @currentId bigint
	SELECT @currentId = MIN(Id) FROM @ClientIdTable

	WHILE @currentId IS NOT NULL
	BEGIN
		EXEC Billing.ReplicateClient @Id = @currentId
		SELECT @currentId = MIN(Id) FROM @ClientIdTable WHERE Id > @currentId
	END

COMMIT TRAN
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH";
            context.Database.ExecuteNonQuery(query);
        }

        private void AlterImportFirmFromXml(IMigrationContext context)
        {
            const string query = @"-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   24.06.2013, a.rechkalov: замена int -> bigint
--   10.09.2013, y.baranihin: dgppid -> id
ALTER PROCEDURE [Integration].[ImportFirmFromXml]
	@Xml [xml] = NULL,
	@ModifiedBy [bigint] = NULL,
	@OwnerCode [bigint] = NULL,
	@EnableReplication [bit] = 1,
	@RegionalTerritoryLocalName nvarchar(255)
WITH EXECUTE AS CALLER
AS
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        IF (@Xml IS NULL)
        BEGIN
            SELECT NULL
            RETURN
        END
            
        DECLARE @xmlHandle int
        EXEC sp_xml_preparedocument @xmlHandle OUTPUT, @xml

        -- @XmlFirmDtos
		DECLARE @XmlFirmDtos TABLE (Code BIGINT NOT NULL, BranchCode INT NOT NULL, Name NVARCHAR(250) NOT NULL, IsArchived BIT NOT NULL, IsHidden BIT NOT NULL)
        INSERT INTO @XmlFirmDtos
        SELECT
        Code,
        BranchCode,
        ISNULL(PromotionalName, Name) AS Name , -- PromotionalName главнее чем Name
        ISNULL(IsArchived, 0) AS IsArchived , -- xsd default value
		ISNULL(IsHidden, 0) AS IsHidden -- xsd default value
        FROM OPENXML (@xmlHandle, 'Root/Firm', 0) WITH (Code BIGINT, BranchCode INT, Name NVARCHAR(250), PromotionalName NVARCHAR(250), IsArchived bit, IsHidden bit) XmlFirmDtos

        -- @Cards
        DECLARE @Cards TABLE (InsertOrder INT IDENTITY(1, 1) NOT NULL, FirmCode BIGINT NOT NULL, Code BIGINT NOT NULL, SortingPosition INT NOT NULL)
        INSERT INTO @Cards
        SELECT FirmCode, Code, 0 FROM OPENXML (@xmlHandle, 'Root/Firm/Card', 1) WITH (FirmCode BIGINT '../@Code', Code BIGINT '@Code')

        UPDATE Cards
        SET SortingPosition = CalculatedSortingPosition
        FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY FirmCode ORDER BY InsertOrder) FROM @Cards) CalculatedCards
        INNER JOIN @Cards Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

        EXEC sp_xml_removedocument @xmlHandle

        -- @FirmDtos
        DECLARE @FirmDtos TABLE
        (
        DgppId BIGINT NOT NULL,
        OrganizationUnitId bigint NOT NULL,
        Name NVARCHAR(250) NOT NULL,
        IsActive BIT NOT NULL,
		ClosedForAscertainment BIT NOT NULL
        )

             -- убеждаемся, что все подразделения определились по DgppId успешно
           declare @InvalidDgppId bigint
           SET @InvalidDgppId = (select top 1 BranchCode from @XmlFirmDtos XmlFirmDtos left join Billing.OrganizationUnits OU ON XmlFirmDtos.BranchCode = OU.DgppId where OU.Id IS NULL)
           if @InvalidDgppId is not null
           begin
               declare @MessageOrganizationUnitInvalidDgppId as nvarchar(max)
               -- закомментированный вариант работает, но только в 2012
               --set @MessageOrganizationUnitInvalidDgppId = concat('Подразделение с кодом ДГПП ', @InvalidDgppId, ' не найдено в системе')
               set @MessageOrganizationUnitInvalidDgppId = 'Подразделение с кодом ДГПП ' + CONVERT(nvarchar(32), @InvalidDgppId) + ' не найдено в системе'
               RAISERROR(@MessageOrganizationUnitInvalidDgppId, 16, 2)
           end

        INSERT INTO @FirmDtos
        SELECT
        DgppId = XmlFirmDtos.Code,
        OrganizationUnitId = (SELECT Id FROM Billing.OrganizationUnits WHERE DgppId = XmlFirmDtos.BranchCode),
        Name = XmlFirmDtos.Name,
        IsActive = ~XmlFirmDtos.IsArchived,
		ClosedForAscertainment = XmlFirmDtos.IsHidden
        FROM @XmlFirmDtos XmlFirmDtos
        -- пропускаем такие фирмы, это временные фирмы inforussia, они нам не нужны
        -- но оставляем уже заведенные
        WHERE NOT(XmlFirmDtos.IsHidden = 1 AND XmlFirmDtos.IsArchived = 1 AND NOT EXISTS(SELECT * FROM @Cards WHERE FirmCode = XmlFirmDtos.Code) AND NOT EXISTS(SELECT * FROM BusinessDirectory.Firms WHERE Id = XmlFirmDtos.Code))

        IF (NOT EXISTS(SELECT 1 FROM @FirmDtos))
        BEGIN
            SELECT NULL
            RETURN
        END

        BEGIN TRAN

        -- update firms using dto
        DECLARE @FrimIds TABLE (Id bigint NOT NULL)

        UPDATE BusinessDirectory.Firms
        SET
            OrganizationUnitId = FirmDtos.OrganizationUnitId,
            Name = FirmDtos.Name,
            IsActive = FirmDtos.IsActive,
            ClosedForAscertainment = FirmDtos.ClosedForAscertainment,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETUTCDATE()
        OUTPUT inserted.Id INTO @FrimIds
        FROM BusinessDirectory.Firms F INNER JOIN @FirmDtos FirmDtos ON F.Id = FirmDtos.DgppId

		-- для удаленных, скрытых фирм, которые были основными, зануляем поле MainFirmId в Клиентах и Сделках
		update Billing.Deals
		set MainFirmId = null
		from Billing.Deals
			inner join BusinessDirectory.Firms on Firms.Id = Deals.MainFirmId
			inner join @FirmDtos as FirmDtos on FirmDtos.DgppId = Firms.Id
		where Firms.IsActive = 0 or Firms.IsDeleted = 1 or Firms.ClosedForAscertainment = 1

		update Billing.Clients
		set MainFirmId = null
		from Billing.Clients
			inner join BusinessDirectory.Firms on Firms.Id = Clients.MainFirmId
			inner join @FirmDtos as FirmDtos on FirmDtos.DgppId = Firms.Id
		where Firms.IsActive = 0 or Firms.IsDeleted = 1 or Firms.ClosedForAscertainment = 1


        IF (EXISTS(SELECT 1 FROM @FirmDtos FirmDtos LEFT JOIN BusinessDirectory.Firms F ON F.Id = FirmDtos.DgppId WHERE F.Id IS NULL))
        BEGIN

            -- заполняем таблицу временных территорий (сложно, надо упростить)
            DECLARE @TemporaryTerritories TABLE (TerritoryId bigint NOT NULL, OrganizationUnitId bigint NOT NULL)
            
            DECLARE @OrganizationUnitIds Shared.OrganizationUnitsTableType
            INSERT INTO @OrganizationUnitIds
            SELECT DISTINCT OrganizationUnitId FROM @FirmDtos OrganizationUnitIds

            INSERT INTO @TemporaryTerritories EXEC Shared.GetTemporaryTerritories @OrganizationUnits = @OrganizationUnitIds, @ModifiedBy = @ModifiedBy, @RegionalTerritoryLocalName = @RegionalTerritoryLocalName

            -- unsert firms using dto
			INSERT INTO BusinessDirectory.Firms (Id, ReplicationCode, Name, UsingOtherMedia, ProductType, MarketType, OrganizationUnitId, TerritoryId, ClosedForAscertainment, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsActive)
            OUTPUT inserted.Id INTO @FrimIds
			SELECT FirmDtos.DgppId, NEWID(), FirmDtos.Name, 0, 0, 0, FirmDtos.OrganizationUnitId, TemporaryTerritories.TerritoryId, FirmDtos.ClosedForAscertainment, @OwnerCode, @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE(), FirmDtos.IsActive
            FROM (SELECT FirmDtos.* FROM @FirmDtos FirmDtos LEFT JOIN BusinessDirectory.Firms F ON F.Id = FirmDtos.DgppId WHERE F.Id IS NULL) FirmDtos
            INNER JOIN @TemporaryTerritories TemporaryTerritories ON TemporaryTerritories.OrganizationUnitId = FirmDtos.OrganizationUnitId
        END

        DECLARE @DeletedAddresses TABLE (Id bigint NOT NULL)  

        -- удаляем ненужные адреса фирмы
        UPDATE BusinessDirectory.FirmAddresses
        SET IsDeleted = 1,
            IsActive = 0,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETUTCDATE()
        OUTPUT inserted.Id INTO @DeletedAddresses
        WHERE
        FirmId IN (SELECT Id FROM @FrimIds)
        AND
        Id NOT IN (SELECT Code FROM @Cards)

        -- удаляем рубрики удаленных адресов
        UPDATE BusinessDirectory.CategoryFirmAddresses
        SET IsDeleted = 1,
            IsActive = 0,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETUTCDATE()
        WHERE
        FirmAddressId IN (SELECT Id FROM @DeletedAddresses)

        -- вяжем адреса с фирмой и проставляем sorting position
        UPDATE FA
        SET	FirmId = F.Id,
            SortingPosition = Cards.SortingPosition,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETUTCDATE()
        FROM BusinessDirectory.FirmAddresses FA
        INNER JOIN @Cards Cards ON FA.Id = Cards.Code
        INNER JOIN BusinessDirectory.Firms F ON F.Id = Cards.FirmCode

    -- определяем первый активный адрес для фирмы, чтобы далее обновить у нее территорию 
        DECLARE @FirstActiveAdressForFirm TABLE(SortingPosition int NOT NULL, FirmId bigint NOT NULL)
        INSERT INTO @FirstActiveAdressForFirm
        SELECT min(cards.SortingPosition), FirmId FROM BusinessDirectory.FirmAddresses FA 
        inner join @Cards cards ON Cards.Code = FA.Id AND FA.IsActive = 1 AND FA.IsDeleted = 0 AND FA.ClosedForAscertainment = 0
        inner join Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        inner join BusinessDirectory.Territories t on t.id = b.TerritoryId 
        inner join BusinessDirectory.Firms f on f.Id = Cards.FirmCode 
            WHERE t.OrganizationUnitId = f.OrganizationUnitId 
        GROUP BY FirmId 

        DECLARE @UpdatedFirms TABLE
            (
                Id bigint NOT NULL
            )

        -- set firm territory
        UPDATE F
        SET F.TerritoryId = B.TerritoryId
        OUTPUT inserted.Id INTO @UpdatedFirms
        FROM @FirmDtos FirmDtos
        INNER JOIN BusinessDirectory.Firms F ON F.Id = FirmDtos.DgppId
        INNER JOIN @Cards Cards ON Cards.FirmCode = FirmDtos.DgppId
        INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.Id = Cards.Code
        INNER JOIN Integration.Buildings B ON B.Code = FA.BuildingCode
        INNER JOIN @FirstActiveAdressForFirm AA ON AA.FirmId = F.Id
        WHERE Cards.SortingPosition = AA.SortingPosition

        -- Выставим региональную территорию тем фирмам, у которых не нашлось подходящаго адреса
            DECLARE @TempTerritoriesTbl TABLE
            (
                TerritoryId bigint, 
                OrganizationUnitId bigint
            )

        INSERT INTO @TempTerritoriesTbl SELECT 
                Id, OrganizationUnitId 
                FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @FirmDtos WHERE Id NOT IN (SELECT Id FROM @UpdatedFirms))
                AND IsActive = 1 AND Name like '%' + @RegionalTerritoryLocalName + '%'

        UPDATE F
        SET F.TerritoryId = AA.TerritoryId	
        FROM BusinessDirectory.Firms F
        INNER JOIN @FirmDtos dto ON F.Id = dto.DgppId
        INNER JOIN @TempTerritoriesTbl AA ON AA.OrganizationUnitId = F.OrganizationUnitId
        WHERE F.Id NOT IN (SELECT Id FROM @UpdatedFirms)
        
        -- update client territory
        DECLARE @ClientIdTable TABLE (Id bigint NOT NULL)

        UPDATE C
        SET C.TerritoryId = F.TerritoryId
        OUTPUT inserted.Id INTO @ClientIdTable
        FROM @FirmDtos FirmDtos
        INNER JOIN BusinessDirectory.Firms F ON F.Id = FirmDtos.DgppId
        INNER JOIN Billing.Clients C ON C.Id = F.ClientId
        INNER JOIN BusinessDirectory.Territories T ON T.Id = C.TerritoryId
        WHERE T.IsActive = 0 AND (C.MainFirmId IS NULL OR C.MainFirmId = F.Id)

        -- replicate clients
        IF(@EnableReplication = 1)
        BEGIN
            DECLARE @currentClientId bigint
            SELECT @currentClientId = MIN(Id) FROM @ClientIdTable
            WHILE @currentClientId IS NOT NULL
            BEGIN
                EXEC Billing.ReplicateClient @Id = @currentClientId
                SELECT @currentClientId = MIN(Id) FROM @ClientIdTable WHERE Id > @currentClientId
            END
        END

        COMMIT TRAN

    END TRY
    BEGIN CATCH
        IF (XACT_STATE() != 0)
            ROLLBACK TRAN

        DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
        SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH";
            context.Database.ExecuteNonQuery(query);
        }
    }
}
