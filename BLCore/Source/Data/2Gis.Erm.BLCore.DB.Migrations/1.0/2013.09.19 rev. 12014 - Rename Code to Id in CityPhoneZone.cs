using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12014, "Переименование Code в Id в таблице CityPhoneZone, изменение типа на bigint, обновление ImportCardsFromXml")]
    public class Migration12014 : TransactedMigration
    {
        #region Alter ImportCardsFromXml

        private const string AlterImportCardsFromXml = @"
-- changes
-- 05.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
-- 05.06.2013, a.rechkalov: добавил условие, чтобы в Integration.Builings не пытался вставиться NULL
-- 24.06.2013, a.rechkalov: замена int -> bigint
-- 05.07.2013, a.rechkalov: слияние с версией из 0.18, из ветки пришло изменение: возврат идентификаторов фирм из хранимки вместо записи их в таблицу
-- 09.07.2013, v.sinitsyn: поправлен вызов STR(FirmCode): для больших значений возвращал '*********************'
-- 27.08.2013, y.baranihin: удалено чтение неиспользуемых полей
-- 10.09.2013, y.baranihin: использование сгенерированних идентификаторов
-- 19.09.2013, a.tukaev: переименование Code в Id в таблице Integration.CityPhoneZone
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
								left join [Integration].CityPhoneZone as zones on ZoneCode = zones.Id  
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

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.CityPhoneZone);
            if (table.Columns["Id"] != null)
            {
                return;
            }

            var codeColumn = table.Columns["Code"];

            var primaryKey = table.Indexes.Cast<Index>().SingleOrDefault(x => x.IndexKeyType == IndexKeyType.DriPrimaryKey);
            if (primaryKey != null && primaryKey.IndexedColumns[0].Name == codeColumn.Name)
            {
                var script = primaryKey.Script();
                primaryKey.Drop();

                codeColumn.DataType = DataType.BigInt;
                codeColumn.Alter();

                context.Database.ExecuteNonQuery(script);
            }

            codeColumn.Rename("Id");

            context.Database.ExecuteNonQuery(AlterImportCardsFromXml);
        }
    }
}