using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4172, "Изменение хранимой процедуры ImportFirmFromXml и создание ImportCardsFromXml")]
    public sealed class Migration4172 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateOrganizationUnitsTableType(context);
            CreateCardsTableType(context);
            CreateContactDTOsTableType(context);
            AlterImportFirmFromXmlProcedure(context);
            RefreshAssemblies(context);
            CreateFirmsForPostIntegrationActivitiesTable(context);
            CreateGetWorkingTimeProcedure(context);
            CreateGetTemporaryTerritoriesProcedure(context);
            CreateImportDepCardsFromXmlProcedure(context);
            CreateImportCardsFromXmlProcedure(context);
            CreatePostIntegrationActivitiesWithFirmsProcedure(context);
        }

        private void AlterImportFirmFromXmlProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string importFirmFromXmlText = @"ALTER PROCEDURE [Integration].[ImportFirmFromXml]
	@Xml [xml] = NULL,
	@ModifiedBy [int] = NULL,
	@OwnerCode [int] = NULL,
	@EnableReplication [bit] = 1
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

	-- @PromotionalFlampLinks
	DECLARE @PromotionalFlampLinks TABLE (FirmCode BIGINT NOT NULL, Value BIT NULL)
	INSERT INTO @PromotionalFlampLinks
	SELECT FirmCode, Value FROM OPENXML (@xmlHandle, 'Root/Firm/Fields/BoolField[@Code=''PromotionalFlampLink'']', 0) WITH (FirmCode BIGINT '../../@Code', Value BIT '@Value')

	-- @FlampLinks
	DECLARE @FlampLinks TABLE (FirmCode BIGINT NOT NULL, Value BIT NULL)
	INSERT INTO @FlampLinks
	SELECT FirmCode, Value FROM OPENXML (@xmlHandle, 'Root/Firm/Fields/BoolField[@Code=''FlampLink'']', 0) WITH (FirmCode BIGINT '../../@Code', Value BIT '@Value')

	-- @XmlFirmDtos
	DECLARE @XmlFirmDtos TABLE (Code BIGINT NOT NULL, BranchCode INT NOT NULL, Name NVARCHAR(250) NOT NULL, IsArchived BIT NOT NULL, IsHidden BIT NOT NULL, FlampLink BIT NULL)
	INSERT INTO @XmlFirmDtos
	SELECT
	Code,
	BranchCode,
	COALESCE(PromotionalName, Name) AS Name , -- PromotionalName главнее чем Name
	COALESCE(IsArchived, 0) AS IsArchived , -- xsd default value
	COALESCE(IsHidden, 0) AS IsHidden, -- xsd default value
	COALESCE(PromotionalFlampLinks.Value, FlampLinks.Value) AS FlampLink -- PromotionalFlampLink главнее чем FlampLink
	FROM OPENXML (@xmlHandle, 'Root/Firm', 0) WITH (Code BIGINT, BranchCode INT, Name NVARCHAR(250), PromotionalName NVARCHAR(250), IsArchived bit, IsHidden bit) XmlFirmDtos
	LEFT JOIN @PromotionalFlampLinks PromotionalFlampLinks ON PromotionalFlampLinks.FirmCode = XmlFirmDtos.Code
	LEFT JOIN @FlampLinks FlampLinks ON FlampLinks.FirmCode = XmlFirmDtos.Code

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
	OrganizationUnitId INT NOT NULL,
	Name NVARCHAR(250) NOT NULL,
	IsActive BIT NOT NULL,
	ClosedForAscertainment BIT NOT NULL,
	FlampLinkMode SMALLINT NOT NULL
	)

	INSERT INTO @FirmDtos
	SELECT
	DgppId = XmlFirmDtos.Code,
	OrganizationUnitId = (SELECT Id FROM Billing.OrganizationUnits WHERE DgppId = XmlFirmDtos.BranchCode),
	Name = XmlFirmDtos.Name,
	IsActive = ~XmlFirmDtos.IsArchived,
	ClosedForAscertainment = XmlFirmDtos.IsHidden,
	FlampLinkMode=
	CASE
		WHEN XmlFirmDtos.FlampLink IS NULL THEN 0 -- FlampAbsence
		WHEN XmlFirmDtos.FlampLink = 0 THEN 1 -- FlampNotPublished
		WHEN XmlFirmDtos.FlampLink = 1 THEN 2 -- FlampPublished
	END
	FROM @XmlFirmDtos XmlFirmDtos
	-- пропускаем такие фирмы, это временные фирмы inforussia, они нам не нужны
	-- но оставляем уже заведенные
	WHERE NOT(XmlFirmDtos.IsHidden = 1 AND XmlFirmDtos.IsArchived = 1 AND NOT EXISTS(SELECT * FROM @Cards WHERE FirmCode = XmlFirmDtos.Code) AND NOT EXISTS(SELECT * FROM BusinessDirectory.Firms WHERE DgppId = XmlFirmDtos.Code))

	IF (NOT EXISTS(SELECT 1 FROM @FirmDtos))
	BEGIN
		SELECT NULL
		RETURN
	END

	BEGIN TRAN

	-- update firms using dto
	DECLARE @FrimIds TABLE (Id INT NOT NULL)

	UPDATE BusinessDirectory.Firms
	SET
		OrganizationUnitId = FirmDtos.OrganizationUnitId,
		Name = FirmDtos.Name,
		IsActive = FirmDtos.IsActive,
		ClosedForAscertainment = FirmDtos.ClosedForAscertainment,
		ShowFlampLink = FirmDtos.FlampLinkMode,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = GETUTCDATE()
	OUTPUT inserted.Id INTO @FrimIds
	FROM BusinessDirectory.Firms F INNER JOIN @FirmDtos FirmDtos ON F.DgppId = FirmDtos.DgppId

	IF (EXISTS(SELECT 1 FROM @FirmDtos FirmDtos LEFT JOIN BusinessDirectory.Firms F ON F.DgppId = FirmDtos.DgppId WHERE F.DgppId IS NULL))
	BEGIN

		-- заполняем таблицу временных территорий (сложно, надо упростить)
		DECLARE @TemporaryTerritories TABLE (TerritoryId INT NOT NULL, OrganizationUnitId INT NOT NULL)
		
		DECLARE @OrganizationUnitIds Shared.OrganizationUnitsTableType
		INSERT INTO @OrganizationUnitIds
		SELECT DISTINCT OrganizationUnitId FROM @FirmDtos OrganizationUnitIds

		INSERT INTO @TemporaryTerritories EXEC Shared.GetTemporaryTerritories @OrganizationUnits = @OrganizationUnitIds, @ModifiedBy = @ModifiedBy, @OwnerCode = @OwnerCode, @EnableReplication = @EnableReplication

		-- unsert firms using dto
		INSERT INTO BusinessDirectory.Firms (DgppId, ReplicationCode, Name, UsingOtherMedia, ProductType, MarketType, OrganizationUnitId, TerritoryId, ClosedForAscertainment, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsActive, ShowFlampLink)
		OUTPUT inserted.Id INTO @FrimIds
		SELECT FirmDtos.DgppId, NEWID(), FirmDtos.Name, 0, 0, 0, FirmDtos.OrganizationUnitId, TemporaryTerritories.TerritoryId, FirmDtos.ClosedForAscertainment, @OwnerCode, @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE(), FirmDtos.IsActive, FirmDtos.FlampLinkMode
		FROM (SELECT FirmDtos.* FROM @FirmDtos FirmDtos LEFT JOIN BusinessDirectory.Firms F ON F.DgppId = FirmDtos.DgppId WHERE F.DgppId IS NULL) FirmDtos
		INNER JOIN @TemporaryTerritories TemporaryTerritories ON TemporaryTerritories.OrganizationUnitId = FirmDtos.OrganizationUnitId
	END

	-- удаляем ненужные адреса фирмы
	UPDATE BusinessDirectory.FirmAddresses
	SET IsDeleted = 1,
		IsActive = 0,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = GETUTCDATE()
	WHERE
	FirmId IN (SELECT Id FROM @FrimIds)
	AND
	DgppId NOT IN (SELECT Code FROM @Cards)

	-- вяжем адреса с фирмой и проставляем sorting position
	UPDATE FA
	SET	FirmId = F.Id,
		SortingPosition = Cards.SortingPosition,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = GETUTCDATE()
	FROM BusinessDirectory.FirmAddresses FA
	INNER JOIN @Cards Cards ON FA.DgppId = Cards.Code
	INNER JOIN BusinessDirectory.Firms F ON F.DgppId = Cards.FirmCode

	SELECT Id FROM @FrimIds

	-- set firm territory
	UPDATE F
	SET F.TerritoryId = B.TerritoryId
	FROM @FirmDtos FirmDtos
	INNER JOIN BusinessDirectory.Firms F ON F.DgppId = FirmDtos.DgppId
	INNER JOIN @Cards Cards ON Cards.FirmCode = FirmDtos.DgppId
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.DgppId = Cards.Code
	INNER JOIN Integration.Buildings B ON B.Code = FA.BuildingCode
	WHERE Cards.SortingPosition = 1

	-- репликация фирм в Dynamics CRM
	IF(@EnableReplication = 1)
	BEGIN
		DECLARE @CurrentFirmId INT
		SELECT @CurrentFirmId = MIN(Id) FROM @FrimIds
		WHILE @CurrentFirmId IS NOT NULL
		BEGIN
			EXEC BusinessDirectory.ReplicateFirm @CurrentFirmId

			SELECT @CurrentFirmId = MIN(Id) FROM @FrimIds WHERE Id > @CurrentFirmId
		END
	END

	-- update client territory
	DECLARE @ClientIdTable TABLE (Id INT NOT NULL)

	UPDATE C
	SET C.TerritoryId = F.TerritoryId
	OUTPUT inserted.Id INTO @ClientIdTable
	FROM @FirmDtos FirmDtos
	INNER JOIN BusinessDirectory.Firms F ON F.DgppId = FirmDtos.DgppId
	INNER JOIN Billing.Clients C ON C.Id = F.ClientId
	INNER JOIN BusinessDirectory.Territories T ON T.Id = C.TerritoryId
	WHERE T.IsActive = 0 AND (C.MainFirmId IS NULL OR C.MainFirmId = F.Id)

	-- replicate clients
	IF(@EnableReplication = 1)
	BEGIN
		DECLARE @currentClientId INT
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
            #endregion

            context.Connection.ExecuteNonQuery(importFirmFromXmlText);
        }

        private void RefreshAssemblies(IMigrationContext context)
        {
            #region Текст запроса
            const string ScriptText = @"
DROP Function [Shared].[ConvertUtcToTimeZone]
GO

DROP Function [Shared].[ConvertUtcToTimeZoneClrProxy]
GO

DROP ASSEMBLY [DoubleGis.Databases.Erm.SqlClr]
GO

Create ASSEMBLY [DoubleGis.Erm.SqlClr] FROM {0} 
GO

CREATE FUNCTION [Shared].[ConvertUtcToTimeZone](@utcDateTime [datetime], @timeZoneId [nvarchar](256))
RETURNS [datetime] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [DoubleGis.Erm.SqlClr].[DoubleGis.Erm.SqlClr.ScalarFunctions].[ConvertUtcToTimeZone]
GO

CREATE FUNCTION [Shared].[FormatPhoneAndFax](@phone [nvarchar](50), @format [nvarchar](50), @zone [nvarchar](50))
RETURNS nvarchar(50) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [DoubleGis.Erm.SqlClr].[DoubleGis.Erm.SqlClr.ScalarFunctions].[FormatPhoneAndFax]
GO
"; 
            #endregion
            byte[] dllAsArray = Resources.DoubleGis_Erm_SqlClr;
            StringBuilder sb = new StringBuilder();

            sb.Append("0x");

            foreach (byte b in dllAsArray)
            {
                sb.Append(b.ToString("X2"));
            }

            string assemblyString = sb.ToString();
            context.Connection.ExecuteNonQuery(string.Format(ScriptText, assemblyString));
        }

        private void CreateFirmsForPostIntegrationActivitiesTable(IMigrationContext context)
        {
            #region Текст процедуры
            const string FirmsForPostIntegrationActivitiesTableText = @"CREATE TABLE [Integration].[FirmsForPostIntegrationActivities](
	[FirmId] [int] NOT NULL,
	[CreatedOn] [datetime2](2) NOT NULL
) ON [PRIMARY]";
            #endregion

            context.Connection.ExecuteNonQuery(FirmsForPostIntegrationActivitiesTableText);
        }

        private void CreateOrganizationUnitsTableType(IMigrationContext context)
        {
            #region Текст процедуры
            const string OrganizationUnitsTableTypeText = @"CREATE TYPE Shared.OrganizationUnitsTableType AS TABLE
  (                     
        [OrganizationUnitId] int NOT NULL
  )";
            #endregion

            context.Connection.ExecuteNonQuery(OrganizationUnitsTableTypeText);
        }

        private void CreateCardsTableType(IMigrationContext context)
        {
            #region Текст процедуры
            const string CardsTableTypeText = @"CREATE TYPE Integration.CardsTableType AS TABLE
  (
	CardCode BIGINT NOT NULL, 
	FirmCode BIGINT NOT NULL, 
	BranchCode INT NOT NULL, 
	CardType nvarchar(3) NOT NULL, 
	BuildingCode BIGINT,
	Address nvarchar(max),
	PromotionalReferencePoint nvarchar(max),
	ReferencePoint nvarchar(max),
	IsCardHiddenOrArchived BIT,
	WorkingTime NVARCHAR(max),
	PaymentMethods NVARCHAR(max),
	IsHidden BIT,
	IsArchived BIT,
	IsDeleted BIT,
	FirmId INT,
	AddressId INT
   )";
            #endregion

            context.Connection.ExecuteNonQuery(CardsTableTypeText);
        }

        private void CreateContactDTOsTableType(IMigrationContext context)
        {
            #region Текст процедуры
            const string ContactDTOsTableTypeText = @"CREATE TYPE Integration.ContactDTOsTableType AS TABLE
  (
	InsertOrder INT NOT NULL,
	CardCode BIGINT NOT NULL,
	ContactName NVARCHAR(250) NOT NULL,
	ContactTypeId INT NOT NULL,
	Contact NVARCHAR(max),
	ZoneCode NVARCHAR(max),
	FormatCode NVARCHAR(max),
	CanReceiveFax BIT,
	ZoneName NVARCHAR(max),
	FormatName NVARCHAR(max),
	IsContactInactive BIT,
	SortingPosition INT,
	AddressId INT
   )";
            #endregion

            context.Connection.ExecuteNonQuery(ContactDTOsTableTypeText);
        }

        private void CreateGetWorkingTimeProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string GetWorkingTimeText = @"CREATE FUNCTION [Shared].[GetWorkingTime](@DayLabel nvarchar(3), @DayFrom Time, @DayTo Time, @BreakFrom Time, @BreakTo Time)
RETURNS nvarchar(max)
AS
BEGIN
DECLARE @DaysOfWeek table(
    Name nvarchar(3) NOT NULL,
	Alias nvarchar(max) NOT NULL);

DECLARE	@dayOff NVARCHAR(max)
DECLARE @roundTheClock NVARCHAR(max)
DECLARE @roundTheClockTimePattern TIME
DECLARE @DayOfWeek NVARCHAR(max)
DECLARE @WorkHours NVARCHAR(max)
DECLARE @BreakHours NVARCHAR(max)
SET @dayOff = 'Не работает'
SET @roundTheClock = 'Круглосуточно'
SET @roundTheClockTimePattern = '00:00:00'

insert into @DaysOfWeek (Name, Alias) Values ('Mon', 'Понедельник');
insert into @DaysOfWeek (Name, Alias) Values ('Tue', 'Вторник');
insert into @DaysOfWeek (Name, Alias) Values ('Wed', 'Среда');
insert into @DaysOfWeek (Name, Alias) Values ('Thu', 'Четверг');
insert into @DaysOfWeek (Name, Alias) Values ('Fri', 'Пятница');
insert into @DaysOfWeek (Name, Alias) Values ('Sat', 'Суббота');
insert into @DaysOfWeek (Name, Alias) Values ('Sun', 'Воскресенье');


SET @DayOfWeek = (SELECT TOP 1 Alias FROM @DaysOfWeek WHERE Name = @DayLabel )

IF @DayFrom IS NULL OR @DayTo IS NULL
	BEGIN
		SET @WorkHours = @DayOff
	END
ELSE
	BEGIN
		IF @DayFrom = @roundTheClockTimePattern AND @DayTo = @roundTheClockTimePattern
			BEGIN
				SET @WorkHours = @roundTheClock
			END
		ELSE
			BEGIN
				SET @WorkHours = CONVERT(VARCHAR(5), @DayFrom, 108) + ' - ' + CONVERT(VARCHAR(5), @DayTo, 108)
			END
	END
IF @BreakFrom IS NOT NULL AND @BreakTo IS NOT NULL
	BEGIN
		SET @BreakHours = ', обед '+ CONVERT(VARCHAR(5), @BreakFrom, 108) + ' - ' + CONVERT(VARCHAR(5), @BreakTo, 108)
	END
ELSE
	BEGIN
		SET @BreakHours = ''
	END
RETURN @DayOfWeek+': '+@WorkHours+@BreakHours+'; '
END";
            #endregion

            context.Connection.ExecuteNonQuery(GetWorkingTimeText);
        }

        private void CreateGetTemporaryTerritoriesProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string GetTemporaryTerritoriesText = @"
DROP PROCEDURE Integration.GetTemporaryTerritory
GO

CREATE Procedure [Shared].[GetTemporaryTerritories](@OrganizationUnits Shared.OrganizationUnitsTableType READONLY, @ModifiedBy Int = NULL, @OwnerCode Int = NULL, @EnableReplication Bit = 1)
AS
BEGIN
	BEGIN TRY

		SET XACT_ABORT ON;

		BEGIN TRAN

		DECLARE @TempTbl TABLE
		(
			TerritoryId int, 
			OrganizationUnitId int,
			OrganizationUnitName nvarchar(max)
		)

		INSERT INTO @TempTbl SELECT 
			Id, OrganizationUnitId, NULL FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @OrganizationUnits) AND IsActive = 1 AND IsDeleted = 0 AND Name like '%Региональная территория%'

		INSERT INTO @TempTbl 
		SELECT 
			NULL, 
			CurrentValues.OrganizationUnitId, 
			Billing.OrganizationUnits.Name
		FROM @OrganizationUnits as CurrentValues 
			inner join Billing.OrganizationUnits ON CurrentValues.OrganizationUnitId = Billing.OrganizationUnits.Id 
		WHERE CurrentValues.OrganizationUnitId NOT IN (SELECT DISTINCT OrganizationUnitId FROM @TempTbl)


		IF EXISTS(SELECT * FROM @TempTbl WHERE TerritoryId IS NULL)
		BEGIN

			DECLARE @TerritoryIds TABLE (Id INT NOT NULL, OrganizationUnitId INT NOT NULL)

			INSERT BusinessDirectory.Territories (DgppId, Name, OrganizationUnitId, ReplicationCode, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
			OUTPUT inserted.Id, inserted.OrganizationUnitId INTO @TerritoryIds
			SELECT NULL, OrganizationUnitName + '. Региональная территория',OrganizationUnitId, NEWID(), @OwnerCode, @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE()
			FROM @TempTbl WHERE TerritoryId IS NULL
		
			UPDATE @TempTbl 
		SET TerritoryId = NewValues.Id
		FROM @TempTbl CurrentValues inner join @TerritoryIds NewValues ON CurrentValues.OrganizationUnitId = NewValues.OrganizationUnitId
				
			-- не забываем репликацию в Dynamics CRM
			IF @EnableReplication = 1
			BEGIN
				DECLARE @TerritoryId INT
				SET @TerritoryId = (SELECT MIN(Id) FROM @TerritoryIds)
				WHILE @TerritoryId IS NOT NULL
				BEGIN
					EXEC BusinessDirectory.ReplicateTerritory @TerritoryId
					SET @TerritoryId = (SELECT MIN(Id) FROM @TerritoryIds Where Id>@TerritoryId)
				END
			END
		END

		COMMIT TRAN

		SELECT TerritoryId, OrganizationUnitId FROM @TempTbl
	END TRY
	BEGIN CATCH
		IF (XACT_STATE() != 0)
			ROLLBACK TRAN

		DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
		SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	END CATCH
END";
            #endregion

            context.Connection.ExecuteNonQuery(GetTemporaryTerritoriesText);
        }

        private void CreateImportDepCardsFromXmlProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string importDepCardsFromXmlText = @"CREATE PROCEDURE [Integration].[ImportDepCardsFromXml]
    @XmlCardTbl Integration.CardsTableType READONLY,
	@XmlContactDtos  Integration.ContactDTOsTableType READONLY,
	@ModifiedBy int = NULL,
	@EnableReplication bit = 1
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
INSERT (CardId, Contact, ContactType, CreatedBy, CreatedOn, SortingPosition)
VALUES (NewValues.CardCode, NewValues.Contact, NewValues.ContactTypeId, @ModifiedBy, GETUTCDATE(), NewValues.SortingPosition);

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
END"; 
            #endregion

            context.Connection.ExecuteNonQuery(importDepCardsFromXmlText);
        }

        private void CreateImportCardsFromXmlProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string importCardsFromXmlText = @"Create PROCEDURE [Integration].[ImportCardsFromXml]
    @Doc xml = NULL, 
    @ModifiedBy int = NULL,
	@OwnerCode int = NULL,
	@EnableReplication bit = 1
AS 
BEGIN

SET XACT_ABORT ON;

BEGIN TRY
	IF (@Doc IS NULL)
	BEGIN
		SELECT NULL
		RETURN
	END

	DECLARE @ContactTypes table(
		ID int NOT NULL,
		Name nvarchar(10));

	insert into @ContactTypes (ID, Name) Values (0, 'None');
	insert into @ContactTypes (ID, Name) Values (1, 'Phone');
	insert into @ContactTypes (ID, Name) Values (2, 'Fax');
	insert into @ContactTypes (ID, Name) Values (3, 'Email');
	insert into @ContactTypes (ID, Name) Values (4, 'Web');
	insert into @ContactTypes (ID, Name) Values (5, 'Icq');
	insert into @ContactTypes (ID, Name) Values (6, 'Skype');
	insert into @ContactTypes (ID, Name) Values (7, 'Other');

	DECLARE @idoc int

    BEGIN TRAN

	EXEC sp_xml_preparedocument @idoc OUTPUT, @Doc

	DECLARE @XmlCardTbl Table (CardCode BIGINT NOT NULL, FirmCode BIGINT NOT NULL, BranchCode INT NOT NULL, CardType nvarchar(3) NOT NULL, BuildingCode BIGINT,
		Address nvarchar(max), PromotionalReferencePoint nvarchar(max), ReferencePoint nvarchar(max), IsCardHiddenOrArchived BIT, WorkingTime NVARCHAR(max), PaymentMethods NVARCHAR(max),
		IsHidden BIT, IsArchived BIT, IsDeleted BIT, FirmId INT, AddressId INT)
	INSERT INTO @XmlCardTbl 
	SELECT	
		CardCode,
		FirmCode,
		BranchCode,
		CardType,
		BuildingCode,
		Address,
		PromotionalReferencePoint,
		ReferencePoint,
		COALESCE(IsCardHidden, 0) | COALESCE(IsCardArchived, 0) | COALESCE(IsCardDeleted, 0),
		NULL,
		NULL,
		COALESCE(IsCardHidden, 0),
		COALESCE(IsCardArchived, 0),
		COALESCE(IsCardDeleted, 0),
		NULL,
		NULL
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
			 Address nvarchar(max) './Address/@Text',
			 PromotionalReferencePoint nvarchar(max) './Address/@PromotionalReferencePoint',
			 ReferencePoint nvarchar(max) './Address/@ReferencePoint')

	UPDATE @XmlCardTbl SET Address = Address + ' — ' + PromotionalReferencePoint WHERE PromotionalReferencePoint IS NOT NULL AND LEN(PromotionalReferencePoint)>0
	UPDATE @XmlCardTbl SET Address = Address + ' — ' + ReferencePoint WHERE (PromotionalReferencePoint IS NULL OR LEN(PromotionalReferencePoint)=0) AND (ReferencePoint IS NOT NULL AND LEN(ReferencePoint)>0)

	DECLARE @XmlContactsTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, ContactName NVARCHAR(250) NOT NULL,
		Contact NVARCHAR(max), ZoneCode NVARCHAR(max),FormatCode NVARCHAR(max), CanReceiveFax BIT, IsContactInactive BIT)
	INSERT INTO @XmlContactsTbl SELECT
		Code,
		ContactName,    
		Value,
		ZoneCode,
		FormatCode,
		CanReceiveFax,
		COALESCE(IsHidden, 0) | COALESCE(IsArchived, 0) | COALESCE(NotPublish, 0)
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

	DECLARE @CategoriesDgppIdsTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL,CardCode BIGINT NOT NULL, CategoryDgppId INT NOT NULL, AddressId INT, SortingPosition INT NOT NULL, CategoryId INT)
	INSERT INTO @CategoriesDgppIdsTbl SELECT
		CardCode,
		RubricCode,
		NULL,
		0,
		NULL
	FROM OPENXML (@idoc, '/Root/Card/Rubrics/Rubric',1)
            WITH (
			 CardCode BIGINT '../../@Code', 
			 RubricCode INT '@Code')

	DECLARE @SchedulesTbl Table (InsertOrder INT IDENTITY(1, 1) NOT NULL, CardCode BIGINT NOT NULL, Name NVARCHAR(max), DayLabel NVARCHAR(3), DayFrom Time, DayTo Time, BreakFrom NVARCHAR(max), BreakTo NVARCHAR(max), SortingPosition INT NOT NULL, WorkingTime NVARCHAR(max))
	INSERT INTO @SchedulesTbl SELECT
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
		COALESCE(types.ID, 7),
		Contact,
		ZoneCode,
		FormatCode,
		CanReceiveFax,
		zones.Name,
		formats.Name,
		IsContactInactive,
		0,
		NULL
	
	FROM @XmlContactsTbl 
				 left join @ContactTypes as types on ContactName = types.Name 
				 left join [Integration].CityPhoneZone as zones on ZoneCode = zones.Code  
				 left join [Integration].ReferenceItems as formats on FormatCode = formats.Code
				
	UPDATE @XmlContactDtos set ContactTypeId = 2 Where ContactTypeId = 1 AND CanReceiveFax = 'True' -- телефон с функцией факса - это факс в понимании ERM
	UPDATE @XmlContactDtos set Contact = [Shared].FormatPhoneAndFax(Contact, FormatName, ZoneName) Where ContactTypeId = 1 OR ContactTypeId = 2 AND FormatName!= NULL

	UPDATE @XmlContactDtos
	SET SortingPosition = CalculatedSortingPosition
	FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @XmlContactDtos) CalculatedCards
	INNER JOIN @XmlContactDtos Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

	DECLARE @DepCards Integration.CardsTableType
	INSERT INTO @DepCards
	SELECT * FROM @XmlCardTbl WHERE CardType = 'DEP'

	DECLARE @DepContacts Integration.ContactDTOsTableType
	INSERT INTO @DepContacts
	SELECT * FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)

	EXEC Integration.ImportDepCardsFromXml @XmlCardTbl = @DepCards, @XmlContactDtos = @DepContacts, @ModifiedBy = @ModifiedBy, @EnableReplication = @EnableReplication

	DELETE FROM @XmlContactDtos WHERE CardCode in (SELECT CardCode FROM @DepCards)
	DELETE FROM @XmlCardTbl WHERE CardType = 'DEP'
	DELETE FROM @PaymentTypesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
	DELETE FROM @SchedulesTbl WHERE CardCode in (SELECT CardCode FROM @DepCards)
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
		UPDATE @XmlCardTbl Set PaymentMethods = @CurrentPaymentMethod WHERE CardCode = @CurrentCardCode
		SET @CurrentCardCode = (SELECT MIN(CardCode) FROM @PaymentTypesTbl WHERE CardCode>@CurrentCardCode)
	END

	-- Проставим WorkTime карточкам
	UPDATE @SchedulesTbl
		SET SortingPosition = CalculatedSortingPosition
	FROM (SELECT InsertOrder, CalculatedSortingPosition = RANK() OVER (PARTITION BY CardCode ORDER BY InsertOrder) FROM @SchedulesTbl) CalculatedCards
		INNER JOIN @SchedulesTbl Cards ON Cards.InsertOrder = CalculatedCards.InsertOrder

	DELETE FROM @SchedulesTbl WHERE SortingPosition > 7 -- Оставляем первое расписание для каждой карточки

	UPDATE @SchedulesTbl SET WorkingTime = Shared.GetWorkingTIme(DayLabel, DayFrom, DayTo, BreakFrom, BreakTo)

	DECLARE @CurrentDayId INT
	DECLARE @CurrentWorkingTime NVARCHAR(max)

	SET @CurrentCardCode = (SELECT MIN(CardCode) FROM @SchedulesTbl)
	WHILE @CurrentCardCode IS NOT NULL
	BEGIN
		SET @CurrentDayId = (SELECT MIN(InsertOrder) FROM @SchedulesTbl WHERE CardCode = @CurrentCardCode)
		SET @CurrentWorkingTime = ''
		WHILE @CurrentDayId IS NOT NULL
		BEGIN
			SET @CurrentWorkingTime = @CurrentWorkingTime + (SELECT WorkingTime FROM @SchedulesTbl WHERE InsertOrder = @CurrentDayId)
			SET @CurrentDayId = (SELECT MIN(InsertOrder) FROM @SchedulesTbl WHERE CardCode = @CurrentCardCode AND InsertOrder>@CurrentDayId)
		END
		UPDATE @XmlCardTbl Set WorkingTime = @CurrentWorkingTime WHERE CardCode = @CurrentCardCode
		SET @CurrentCardCode = (SELECT MIN(CardCode) FROM @SchedulesTbl WHERE CardCode>@CurrentCardCode)
	END

	DECLARE @TerritoriesTbl TABLE
(
	TerritoryId INT NOT NULL,
	BranchCode INT NOT NULL
)

	DECLARE @BranchCodes Shared.OrganizationUnitsTableType 
	INSERT INTO @BranchCodes SELECT DISTINCT BranchCode FROM @XmlCardTbl

	INSERT INTO @TerritoriesTbl EXEC Shared.GetTemporaryTerritories @OrganizationUnits = @BranchCodes, @ModifiedBy = @ModifiedBy, @OwnerCode = @ModifiedBy, @EnableReplication = @EnableReplication

	DECLARE @FirmsTbl TABLE
(
	FirmCode BIGINT NULL,
	BranchCode INT NULL,
	FirmId INT,
	TerritoryId INT NOT NULL
)

	INSERT INTO @FirmsTbl
	SELECT DISTINCT CardsTable.FirmCode, CardsTable.BranchCode, BusinessDirectory.Firms.Id, Territories.TerritoryId FROM @XmlCardTbl as CardsTable 
	left join BusinessDirectory.Firms ON CardsTable.FirmCode = BusinessDirectory.Firms.DgppId 
	inner join @TerritoriesTbl as Territories ON CardsTable.BranchCode = Territories.BranchCode 
	
	-- Создаем пустые фирмы, если это необходимо
	IF EXISTS (SELECT * FROM @FirmsTbl WHERE FirmId IS NULL)
	BEGIN
		DECLARE @FirmIds TABLE (Id INT NOT NULL, FirmCode BIGINT NOT NULL)

		INSERT BusinessDirectory.Firms(DgppId, Name, TerritoryId, OrganizationUnitId, UsingOtherMedia, ProductType, MarketType, OwnerCode, IsActive, ClosedForAscertainment, ReplicationCode, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
		OUTPUT inserted.Id, inserted.DgppId INTO @FirmIds
		SELECT FirmCode, 'Пустая фирма #' + LTRIM(STR(FirmCode)),TerritoryId, BranchCode, 0, 0, 0, @OwnerCode, 0, 1, NEWID(), @ModifiedBy, GETUTCDATE(), @ModifiedBy, GETUTCDATE()
		FROM @FirmsTbl WHERE FirmId IS NULL
		
		UPDATE @FirmsTbl 
		SET FirmId = NewValues.Id
		FROM @FirmsTbl CurrentValues inner join @FirmIds NewValues ON CurrentValues.FirmCode = NewValues.FirmCode
		
		---- не забываем репликацию в Dynamics CRM
		IF @EnableReplication = 1
		BEGIN
			DECLARE @FirmId INT
			SET @FirmId = (SELECT MIN(Id) FROM @FirmIds)
			WHILE @FirmId IS NOT NULL
			BEGIN
				EXEC BusinessDirectory.ReplicateFirm @FirmId
				SET @FirmId = (SELECT MIN(Id) FROM @FirmIds Where Id>@FirmId)
			END
		END
	END

	UPDATE @XmlCardTbl
	SET FirmId = Firms.FirmId
	FROM @XmlCardTbl Cards
	INNER JOIN @FirmsTbl Firms ON Cards.FirmCode = Firms.FirmCode

	--Обновляем здания
	INSERT INTO Integration.Buildings(Code, TerritoryId, IsDeleted)
	SELECT DISTINCT CardsTable.BuildingCode, Territories.TerritoryId, 0 FROM  @XmlCardTbl as CardsTable 
	inner join @TerritoriesTbl as Territories ON CardsTable.BranchCode = Territories.BranchCode WHERE CardsTable.BuildingCode NOT IN (SELECT DISTINCT Code FROM Integration.Buildings)

	--Обновляем адреса
	DECLARE @AddressIds TABLE (Id INT NOT NULL)

	MERGE BusinessDirectory.FirmAddresses AS CurrentValues
	USING @XmlCardTbl AS NewValues 
	ON (CurrentValues.DgppId = NewValues.CardCode) 
	WHEN MATCHED THEN 
	UPDATE SET 
		CurrentValues.Address = NewValues.Address,
		CurrentValues.ClosedForAscertainment = NewValues.IsHidden,
		CurrentValues.IsActive = ~NewValues.IsArchived,
		CurrentValues.IsDeleted = NewValues.IsDeleted,
		CurrentValues.PaymentMethods = NewValues.PaymentMethods,
		CurrentValues.WorkingTime = NewValues.WorkingTime,
		CurrentValues.BuildingCode = NewValues.BuildingCode,
		CurrentValues.ModifiedBy = @ModifiedBy,
		CurrentValues.ModifiedOn = GETUTCDATE()
	WHEN NOT MATCHED BY TARGET THEN 
	INSERT (DgppId, Address, ClosedForAscertainment, IsActive, IsDeleted, PaymentMethods, WorkingTime, BuildingCode ,CreatedBy, CreatedOn, ReplicationCode, FirmId, OwnerCode, ModifiedBy, ModifiedOn) 
	VALUES (NewValues.CardCode, NewValues.Address,NewValues.IsHidden,~NewValues.IsArchived, NewValues.IsDeleted, NewValues.PaymentMethods, 
	NewValues.WorkingTime, NewValues.BuildingCode, @ModifiedBy, GETUTCDATE(), NEWID(), NewValues.FirmId, @OwnerCode,  @ModifiedBy, GETUTCDATE())
	OUTPUT inserted.Id INTO @AddressIds;

	-- не забываем репликацию в Dynamics CRM
	IF @EnableReplication = 1
	BEGIN
		DECLARE @AddressId INT
		SET @AddressId = (SELECT MIN(Id) FROM @AddressIds)
		WHILE @AddressId IS NOT NULL
		BEGIN
			EXEC BusinessDirectory.ReplicateFirmAddress @AddressId
			SET @AddressId = (SELECT MIN(Id) FROM @AddressIds Where Id>@AddressId)
		END
	END

	--Проставляем адреса карточкам, контактам и рубрикам
	UPDATE @XmlCardTbl
	SET 
	AddressId = Addresses.Id
	FROM @XmlCardTbl Cards INNER JOIN BusinessDirectory.FirmAddresses Addresses ON Cards.CardCode = Addresses.DgppId 

	UPDATE @XmlContactDtos
	SET 
	AddressId = Addresses.AddressId
	FROM @XmlContactDtos Contacts INNER JOIN @XmlCardTbl Addresses ON Contacts.CardCode = Addresses.CardCode

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
	ON CurrentValues.FirmAddressId = NewValues.AddressId  AND CurrentValues.SortingPosition = NewValues.SortingPosition
	WHEN MATCHED THEN 
	UPDATE SET CurrentValues.IsActive = 1, CurrentValues.IsDeleted = 0, CurrentValues.SortingPosition = NewValues.SortingPosition, CurrentValues.ModifiedBy = @ModifiedBy, CurrentValues.ModifiedOn = GETUTCDATE()
	WHEN NOT MATCHED BY TARGET THEN 
	INSERT (CategoryId, FirmAddressId, IsActive, IsDeleted, CreatedBy, CreatedOn, OwnerCode, SortingPosition, ModifiedBy, ModifiedOn) 
	VALUES (NewValues.CategoryId, NewValues.AddressId, 1,0,@ModifiedBy, GETUTCDATE(), @OwnerCode, NewValues.SortingPosition, @ModifiedBy, GETUTCDATE());

	UPDATE BusinessDirectory.CategoryFirmAddresses 
	SET IsDeleted = 1, ModifiedBy = @ModifiedBy, ModifiedOn = GETUTCDATE()
	WHERE Id IN
	(
		SELECT Id FROM BusinessDirectory.CategoryFirmAddresses AS CurrentValues left join @CategoriesDgppIdsTbl AS NewValues 
		ON CurrentValues.FirmAddressId = NewValues.AddressId AND CurrentValues.SortingPosition = NewValues.SortingPosition WHERE CurrentValues.FirmAddressId IN (SELECT AddressId FROM @XmlCardTbl) AND NewValues.CardCode IS NULL
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
        WHERE CurrentContacts.FirmAddressId in (SELECT DISTINCT AddressId FROM @XmlCardTbl) AND NewValues.CardCode is NULL
    )

	INSERT INTO Integration.FirmsForPostIntegrationActivities SELECT FirmId, GETUTCDATE() FROM @XmlCardTbl

    COMMIT TRAN
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END";
            #endregion

            context.Connection.ExecuteNonQuery(importCardsFromXmlText);
        }

        private void CreatePostIntegrationActivitiesWithFirmsProcedure(IMigrationContext context)
        {
            #region Текст процедуры
            const string importCardsFromXmlText = @"CREATE PROCEDURE [Integration].[PostIntegrationActivitiesWithFirms]
AS
BEGIN

SET XACT_ABORT ON;

BEGIN TRY;

-- Удалаяем дубли фирм, нуждающихся в обработке
WITH CTE AS (
	SELECT FirmId, ROW_NUMBER() OVER(PARTITION BY FirmId ORDER BY FirmId, CreatedOn ASC) rnk
	FROM Integration.FirmsForPostIntegrationActivities
	 )
DELETE FROM CTE
WHERE rnk > 1;

--Выгружаем заказы, связанные с этими фирмами
BEGIN TRAN
DECLARE @ArchiveWorkflowStepId int
SET @ArchiveWorkflowStepId = 6
DECLARE @FakeField int
UPDATE Billing.Orders 
SET @FakeField = 1
WHERE WorkflowStepId != @ArchiveWorkflowStepId AND FirmId in (SELECT FirmId FROM Integration.FirmsForPostIntegrationActivities)
DELETE FROM Integration.FirmsForPostIntegrationActivities
COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END";
            #endregion

            context.Connection.ExecuteNonQuery(importCardsFromXmlText);
        }
    }
}