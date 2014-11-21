﻿using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6820, "При удалении адресов фирмы, также удаляем рубрики, связанные с этими адресами")]
    public sealed class Migration6820 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"ALTER PROCEDURE [Integration].[ImportFirmFromXml]
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

	DECLARE @DeletedAddresses TABLE (Id INT NOT NULL)  

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
	DgppId NOT IN (SELECT Code FROM @Cards)

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
	INNER JOIN @Cards Cards ON FA.DgppId = Cards.Code
	INNER JOIN BusinessDirectory.Firms F ON F.DgppId = Cards.FirmCode

-- определяем первый активный адрес для фирмы, чтобы далее обновить у нее территорию 
	DECLARE @FirstActiveAdressForFirm TABLE(SortingPosition int NOT NULL, FirmId int NOT NULL)
	INSERT INTO @FirstActiveAdressForFirm
	SELECT min(cards.SortingPosition), FirmId FROM BusinessDirectory.FirmAddresses FA 
	inner join @Cards cards ON Cards.Code = FA.DgppId AND FA.IsActive = 1 
	inner join Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
	GROUP BY FirmId 

	-- set firm territory
	UPDATE F
	SET F.TerritoryId = B.TerritoryId
	FROM @FirmDtos FirmDtos
	INNER JOIN BusinessDirectory.Firms F ON F.DgppId = FirmDtos.DgppId
	INNER JOIN @Cards Cards ON Cards.FirmCode = FirmDtos.DgppId
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.DgppId = Cards.Code
	INNER JOIN Integration.Buildings B ON B.Code = FA.BuildingCode
	INNER JOIN @FirstActiveAdressForFirm AA ON AA.FirmId = F.Id
	WHERE Cards.SortingPosition = AA.SortingPosition

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

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
