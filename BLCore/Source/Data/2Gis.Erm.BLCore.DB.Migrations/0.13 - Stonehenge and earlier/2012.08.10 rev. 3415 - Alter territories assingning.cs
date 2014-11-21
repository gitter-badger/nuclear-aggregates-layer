using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3415, "Изменение интеграцию в части импорта территорий")]
    public sealed class Migration3415 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddFirmAddressBuildingsForeignKey(context);
            UpdateFirmsTerritories(context);
            AlterImportFirmFromXml(context);
            AlterUpdateFirmAddressBuildings(context);
            AlterUpdateBuildings(context);
            DropUpdateFirmsTerritoriesSp(context);
            DropDirtyFirms(context);
        }

        private static void UpdateFirmsTerritories(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
	        UPDATE F
	        SET F.TerritoryId = B.TerritoryId
	        FROM (SELECT * FROM Integration.Buildings WHERE TerritoryId IS NOT NULL) B
	        INNER JOIN Integration.FirmAddressBuildings FAB ON FAB.BuildingCode = B.Code
	        INNER JOIN BusinessDirectory.FirmAddresses FA ON FAB.FirmAddressId = FA.Id
	        INNER JOIN BusinessDirectory.Firms F ON FA.FirmId = F.Id");
        }

        private static void DropDirtyFirms(IMigrationContext context)
        {
            var table = context.Database.Tables["DirtyFirms", ErmSchemas.Integration];

            if (table == null)
                return;

            table.Drop();
        }

        private static void DropUpdateFirmsTerritoriesSp(IMigrationContext context)
        {
            const string updateFirmsTerritories = "UpdateFirmsTerritories";

            var updateFirmsTerritoriesSp = context.Database.StoredProcedures[updateFirmsTerritories, ErmSchemas.Integration];

            if (updateFirmsTerritoriesSp == null)
                return;

            updateFirmsTerritoriesSp.Drop();
        }

        private static void AlterUpdateBuildings(IMigrationContext context)
        {
            const string updateBuildings = "UpdateBuildings";
            const string updateBuildingsText = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRAN
	
	DECLARE @docHandle INT

	EXEC sp_xml_preparedocument @docHandle OUTPUT, @buildingsXml

	DECLARE @xmlBuildings TABLE (Code BIGINT NOT NULL, SaleTerritoryCode BIGINT, IsDeleted BIT NOT NULL)
	INSERT INTO @xmlBuildings
	SELECT
	Code,
	SaleTerritoryCode,
	COALESCE(IsDeleted, 0) AS IsDeleted -- xsd default value
	FROM OPENXML(@docHandle, '/buildings/building', 1) WITH (Code BIGINT, SaleTerritoryCode BIGINT, IsDeleted BIT)

	DECLARE @buildings TABLE(Code BIGINT NOT NULL, TerritoryId INT, IsDeleted BIT NOT NULL)
	INSERT INTO @buildings
	SELECT
	xmlBuildings.Code,
	T.Id,
	xmlBuildings.IsDeleted
	FROM @xmlBuildings xmlBuildings
	LEFT JOIN BusinessDirectory.Territories T ON T.DgppId = xmlBuildings.SaleTerritoryCode

	EXEC sp_xml_removedocument @docHandle
	
	-- delete buildings that marked as deleted
	DELETE FAB FROM Integration.FirmAddressBuildings FAB INNER JOIN Integration.Buildings B ON FAB.BuildingCode = B.Code WHERE B.Code IN (SELECT Code FROM @buildings WHERE IsDeleted = 1)
	DELETE FROM Integration.Buildings WHERE Code IN (SELECT Code FROM @buildings WHERE IsDeleted = 1)

	-- update building territory
	UPDATE B
	SET B.TerritoryId = BT.TerritoryID
	FROM Integration.Buildings B
	INNER JOIN @buildings BT ON BT.Code = B.Code
	WHERE BT.TerritoryId IS NOT NULL AND IsDeleted = 0
	
	-- insert new buildings
	INSERT INTO Integration.Buildings (Code, TerritoryId)
	SELECT BT.Code, BT.TerritoryId
	FROM @buildings BT
	WHERE BT.TerritoryId IS NOT NULL AND IsDeleted = 0 AND NOT EXISTS (SELECT 1 FROM Integration.Buildings WHERE Code = BT.Code)
	
	-- update firm territory
	UPDATE F
	SET F.TerritoryId = B.TerritoryId
	FROM (SELECT * FROM Integration.Buildings WHERE Code IN (SELECT Code FROM @buildings WHERE TerritoryId IS NOT NULL AND IsDeleted = 0)) B
	INNER JOIN Integration.FirmAddressBuildings FAB ON FAB.BuildingCode = B.Code
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FAB.FirmAddressId = FA.Id
	INNER JOIN BusinessDirectory.Firms F ON FA.FirmId = F.Id

COMMIT TRAN
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH";

            var updateBuildingsSp = context.Database.StoredProcedures[updateBuildings, ErmSchemas.Integration];
            updateBuildingsSp.TextBody = updateBuildingsText;
            updateBuildingsSp.Alter();
        }

        private static void AlterUpdateFirmAddressBuildings(IMigrationContext context)
        {
            const string updateFirmAddressBuildings = "UpdateFirmAddressBuildings";
            const string updateFirmAddressBuildingsText = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRAN

	DECLARE @docHandle INT

	EXEC sp_xml_preparedocument @docHandle OUTPUT, @linksXml

	DECLARE @links TABLE(FirmAddressId INT NOT NULL, BuildingCode BIGINT)

	INSERT INTO @links
	SELECT FA.Id, L.BuildingCode
	FROM OPENXML(@docHandle, '/links/link', 1) WITH (FirmAddressDgppId BIGINT, BuildingCode BIGINT) L
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.DgppId = L.FirmAddressDgppId

	EXEC sp_xml_removedocument @docHandle

	-- delete redundant relations
	DELETE FAB
	FROM Integration.FirmAddressBuildings FAB
	INNER JOIN @links L ON L.FirmAddressId = FAB.FirmAddressId
	WHERE L.BuildingCode IS NULL
	
	-- update existing relations
	UPDATE FAB
	SET FAB.BuildingCode = L.BuildingCode
	FROM Integration.FirmAddressBuildings FAB
	INNER JOIN
	@links L ON L.FirmAddressId = FAB.FirmAddressId
	
	-- insert new relations
	INSERT INTO Integration.Buildings (Code)
	SELECT BuildingCode FROM @links L
	WHERE L.BuildingCode IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Integration.Buildings WHERE Code = L.BuildingCode)

	INSERT INTO Integration.FirmAddressBuildings (FirmAddressId, BuildingCode)
	SELECT L.FirmAddressId, L.BuildingCode
	FROM @links L
	WHERE NOT EXISTS (SELECT * FROM Integration.FirmAddressBuildings FAB WHERE FAB.FirmAddressId = L.FirmAddressId)
	AND L.BuildingCode IS NOT NULL
	
COMMIT TRAN
END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH";

            var updateFirmAddressBuildingsSp = context.Database.StoredProcedures[updateFirmAddressBuildings, ErmSchemas.Integration];
            updateFirmAddressBuildingsSp.TextBody = updateFirmAddressBuildingsText;
            updateFirmAddressBuildingsSp.Alter();
        }

        private static void AlterImportFirmFromXml(IMigrationContext context)
        {
            const string importFirmFromXml = "ImportFirmFromXml";
            const string importFirmFromXmlText = @"
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

	-- определяем FlampLink
	DECLARE @XmlPromotionalFlampLink BIT, @XmlFlampLink BIT
	SELECT @XmlPromotionalFlampLink = Value FROM OPENXML (@xmlHandle, 'Firm/Fields/BoolField[@Code=''PromotionalFlampLink'']', 0) WITH (Value bit '@Value')
	SELECT @XmlFlampLink = Value FROM OPENXML (@xmlHandle, 'Firm/Fields/BoolField[@Code=''FlampLink'']', 0) WITH (Value bit '@Value')

	-- заполняем XML DTO (учитывая XSD default values)
	DECLARE @XmlFirmDto TABLE
	(
	Code BIGINT NOT NULL,
	BranchCode INT NOT NULL,
	Name NVARCHAR(250) NOT NULL,
	IsArchived BIT NOT NULL,
	IsHidden BIT NOT NULL,
	FlampLink BIT
	)

	INSERT INTO @XmlFirmDto
	SELECT
	Code,
	BranchCode,
	COALESCE(PromotionalName, Name) AS Name , -- PromotionalName главнее чем Name
	COALESCE(IsArchived, 0) AS IsArchived , -- xsd default value
	COALESCE(IsHidden, 0) AS IsHidden, -- xsd default value
	COALESCE(@XmlPromotionalFlampLink, @XmlFlampLink) AS FlampLink -- PromotionalFlampLink главнее чем FlampLink
	FROM OPENXML (@xmlHandle, 'Firm', 0) WITH (Code BIGINT, BranchCode INT, Name NVARCHAR(250), PromotionalName NVARCHAR(250), IsArchived bit, IsHidden bit)

	DECLARE @XmlFirmCards TABLE (SortingPosition INT NOT NULL IDENTITY(1,1), Code BIGINT NOT NULL)
	INSERT INTO @XmlFirmCards SELECT * FROM OPENXML (@xmlHandle, 'Firm/Card', 1) WITH (Code BIGINT '@Code')

	EXEC sp_xml_removedocument @xmlHandle

	-- заполняем DTO (учитывая внутренние особенности ERM)
	DECLARE @FirmDto TABLE
	(
	DgppId BIGINT NOT NULL,
	OrganizationUnitId INT NOT NULL,
	Name NVARCHAR(250) NOT NULL,
	IsActive BIT NOT NULL,
	ClosedForAscertainment BIT NOT NULL,
	FlampLinkMode SMALLINT NOT NULL
	)

	INSERT INTO @FirmDto
	SELECT
	DgppId = XmlFirmDto.Code,
	OrganizationUnitId = (SELECT Id FROM Billing.OrganizationUnits WHERE DgppId = XmlFirmDto.BranchCode),
	Name = XmlFirmDto.Name,
	IsActive = ~XmlFirmDto.IsArchived,
	ClosedForAscertainment = XmlFirmDto.IsHidden,
	FlampLinkMode=
	CASE
		WHEN XmlFirmDto.FlampLink IS NULL THEN 0 -- FlampAbsence
		WHEN XmlFirmDto.FlampLink = 0 THEN 1 -- FlampNotPublished
		WHEN XmlFirmDto.FlampLink = 1 THEN 2 -- FlampPublished
	END
	FROM @XmlFirmDto XmlFirmDto
	-- пропускаем такие фирмы, это временные фирмы inforussia, они нам не нужны
	WHERE NOT(XmlFirmDto.IsHidden = 1 AND XmlFirmDto.IsArchived = 1 AND NOT EXISTS(SELECT * FROM @XmlFirmCards))

	IF (NOT EXISTS(SELECT 1 FROM @FirmDto))
	BEGIN
		SELECT NULL
		RETURN
	END

	BEGIN TRAN

	-- изменяем или добавляем фирму используя DTO
	DECLARE @FrimIdTable TABLE (Id INT NOT NULL)
	
	IF (EXISTS(SELECT 1 FROM BusinessDirectory.Firms F INNER JOIN @FirmDto FirmDto ON F.DgppId = FirmDto.DgppId))
		UPDATE BusinessDirectory.Firms
		SET
			OrganizationUnitId = FirmDto.OrganizationUnitId,
			Name = FirmDto.Name,
			IsActive = FirmDto.IsActive,
			ClosedForAscertainment = FirmDto.ClosedForAscertainment,
			ShowFlampLink = FirmDto.FlampLinkMode,
			ModifiedBy = @ModifiedBy,
			ModifiedOn = GETUTCDATE()
		OUTPUT inserted.Id INTO @FrimIdTable
		FROM BusinessDirectory.Firms F INNER JOIN @FirmDto FirmDto ON F.DgppId = FirmDto.DgppId
	ELSE BEGIN
		DECLARE @OrganizationUnitId INT
		SELECT @OrganizationUnitId = OrganizationUnitId FROM @FirmDto

		DECLARE @TemporaryTerritoryIds TABLE (Id INT NOT NULL)
		INSERT @TemporaryTerritoryIds EXEC Integration.GetTemporaryTerritory @OrganizationUnitId = @OrganizationUnitId, @ModifiedBy = @ModifiedBy, @OwnerCode = @OwnerCode

		INSERT INTO BusinessDirectory.Firms(DgppId, ReplicationCode, Name, UsingOtherMedia, ProductType, MarketType, OrganizationUnitId, TerritoryId, ClosedForAscertainment, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsActive, ShowFlampLink)
		OUTPUT inserted.Id INTO @FrimIdTable
		SELECT FirmDto.DgppId, NEWID(), FirmDto.Name, 0, 0, 0, FirmDto.OrganizationUnitId, (SELECT Id FROM @TemporaryTerritoryIds), FirmDto.ClosedForAscertainment, @OwnerCode, @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE(), FirmDto.IsActive, FirmDto.FlampLinkMode
		FROM @FirmDto FirmDto
	END

	-- не забываем репликацию в Dynamics CRM
	DECLARE @FirmId INT
	SELECT @FirmId = Id FROM @FrimIdTable;
	EXEC BusinessDirectory.ReplicateFirm @FirmId

	-- удаляем ненужные адреса фирмы
	UPDATE BusinessDirectory.FirmAddresses
	SET IsDeleted = 1,
		IsActive = 0,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = GETUTCDATE()
	WHERE
	FirmId IN (SELECT Id FROM @FrimIdTable)
	AND
	DgppId NOT IN (SELECT Code FROM @XmlFirmCards)

	-- вяжем адреса с фирмой и проставляем sorting position
	UPDATE BusinessDirectory.FirmAddresses
	SET	FirmId = (SELECT Id FROM @FrimIdTable),
		SortingPosition = XmlFirmCards.SortingPosition,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = GETUTCDATE()
	FROM BusinessDirectory.FirmAddresses FA
	INNER JOIN @XmlFirmCards XmlFirmCards
	ON FA.DgppId = XmlFirmCards.Code

	SELECT @FirmId

	COMMIT TRAN

END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH";

            var importFirmFromXmlSp = context.Database.StoredProcedures[importFirmFromXml, ErmSchemas.Integration];
            importFirmFromXmlSp.TextBody = importFirmFromXmlText;
            importFirmFromXmlSp.Alter();
        }

        private static void AddFirmAddressBuildingsForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_FirmAddressBuildings_Buildings";

            var table = context.Database.Tables["FirmAddressBuildings", ErmSchemas.Integration];
            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey != null)
                return;

            context.Database.ExecuteNonQuery(@"
            INSERT Integration.Buildings (Code)
            SELECT DISTINCT BuildingCode FROM Integration.FirmAddressBuildings WHERE BuildingCode NOT IN (SELECT Code FROM Integration.Buildings)"
            );

            // create foreign key
            foreignKey = new ForeignKey(table, foreignKeyName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "BuildingCode", "Code"));
            foreignKey.ReferencedTable = "Buildings";
            foreignKey.ReferencedTableSchema = ErmSchemas.Integration;
            foreignKey.Create();
        }
    }
}