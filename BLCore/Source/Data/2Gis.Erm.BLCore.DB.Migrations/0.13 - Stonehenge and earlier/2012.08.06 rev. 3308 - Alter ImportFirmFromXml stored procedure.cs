using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3308, "Изменение хранимых процедуры ImportFirmFromXml и GetTemporaryTerritory")]
    public sealed class Migration3308 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddParametersToImportFirmFromXmlSp(context);
            AlterImportFirmFromXmlSp(context);

            AddParametersToGetTemporaryTerritorySp(context);
            AlterGetTemporaryTerritorySp(context);
        }

        private static void AddParametersToGetTemporaryTerritorySp(IMigrationContext context)
        {
            const string getTemporaryTerritory = "GetTemporaryTerritory";
            const string modifiedBy = "@ModifiedBy";
            const string ownerCode = "@OwnerCode";

            var getTemporaryTerritorySp = context.Database.StoredProcedures[getTemporaryTerritory, ErmSchemas.Integration];
            getTemporaryTerritorySp.TextMode = false;

            var modifiedByParam = getTemporaryTerritorySp.Parameters[modifiedBy];
            if (modifiedByParam == null)
            {
                modifiedByParam = new StoredProcedureParameter(getTemporaryTerritorySp, modifiedBy, DataType.Int) { DefaultValue = "0" };
                getTemporaryTerritorySp.Parameters.Add(modifiedByParam);
                getTemporaryTerritorySp.Alter();
            }

            var ownerCodeParam = getTemporaryTerritorySp.Parameters[ownerCode];
            if (ownerCodeParam == null)
            {
                ownerCodeParam = new StoredProcedureParameter(getTemporaryTerritorySp, ownerCode, DataType.Int) { DefaultValue = "0" };
                getTemporaryTerritorySp.Parameters.Add(ownerCodeParam);
                getTemporaryTerritorySp.Alter();
            }
        }

        private static void AddParametersToImportFirmFromXmlSp(IMigrationContext context)
        {
            const string importFirmFromXml = "ImportFirmFromXml";
            const string modifiedBy = "@ModifiedBy";
            const string ownerCode = "@OwnerCode";

            var importFirmFromXmlSp = context.Database.StoredProcedures[importFirmFromXml, ErmSchemas.Integration];
            importFirmFromXmlSp.TextMode = false;

            var modifiedByParam = importFirmFromXmlSp.Parameters[modifiedBy];
            if (modifiedByParam == null)
            {
                modifiedByParam = new StoredProcedureParameter(importFirmFromXmlSp, modifiedBy, DataType.Int) {DefaultValue = "0"};
                importFirmFromXmlSp.Parameters.Add(modifiedByParam);
                importFirmFromXmlSp.Alter();
            }

            var ownerCodeParam = importFirmFromXmlSp.Parameters[ownerCode];
            if (ownerCodeParam == null)
            {
                ownerCodeParam = new StoredProcedureParameter(importFirmFromXmlSp, ownerCode, DataType.Int) { DefaultValue = "0" };
                importFirmFromXmlSp.Parameters.Add(ownerCodeParam);
                importFirmFromXmlSp.Alter();
            }
        }

        private static void AlterImportFirmFromXmlSp(IMigrationContext context)
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

	-- mark firm as dirty
	INSERT INTO Integration.DirtyFirms (FirmId)
	SELECT * FROM @FrimIdTable FrimIdTable
	WHERE Id NOT IN (SELECT FirmId FROM [Integration].[DirtyFirms])

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

	EXEC sp_xml_removedocument @xmlHandle

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

        private static void AlterGetTemporaryTerritorySp(IMigrationContext context)
        {
            const string getTemporaryTerritory = "GetTemporaryTerritory";

            const string getTemporaryTerritoryText = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY

	IF (@OrganizationUnitId IS NULL)
	BEGIN
		SELECT NULL
		RETURN
	END

	-- плохо использовать понятие временной территории, потом надо избавиться от этого
	DECLARE @TemporaryTerritoryName NVARCHAR(255)
	SELECT @TemporaryTerritoryName = Name + '. Временная территория' FROM Billing.OrganizationUnits WHERE Id = @OrganizationUnitId

	DECLARE @TerritoryId INT
	SELECT @TerritoryId = Id FROM BusinessDirectory.Territories WHERE NAME = @TemporaryTerritoryName

	IF (@TerritoryId IS NULL)
	BEGIN
		BEGIN TRAN

		DECLARE @TerritoryIds TABLE (Id INT NOT NULL)

		INSERT BusinessDirectory.Territories (DgppId, Name, OrganizationUnitId, ReplicationCode, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
		OUTPUT inserted.Id INTO @TerritoryIds
		VALUES (NULL, @TemporaryTerritoryName, @OrganizationUnitId, NEWID(), @OwnerCode, @ModifiedBy, @ModifiedBy, GETUTCDATE(), GETUTCDATE())

		-- не забываем репликацию в Dynamics CRM
		SELECT @TerritoryId = Id FROM @TerritoryIds
		EXEC BusinessDirectory.ReplicateTerritory @TerritoryId

		COMMIT TRAN
	END

	SELECT @TerritoryId

END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH";

            var getTemporaryTerritorySp = context.Database.StoredProcedures[getTemporaryTerritory, ErmSchemas.Integration];
            getTemporaryTerritorySp.TextBody = getTemporaryTerritoryText;
            getTemporaryTerritorySp.Alter();
        }
    }
}