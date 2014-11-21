using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11179, "Alter на UpdateBuildings")]
    public sealed class Migration11179 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["UpdateBuildings", ErmSchemas.Integration];
            sp.TextBody = @"SET NOCOUNT ON;
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
	DECLARE @FirmsToUpdate TABLE (Id bigint NOT NULL, OrganizationUnitId bigint NOT NULL, DgppId BIGINT NOT NULL)
    DECLARE @UpdatedFirms TABLE (Id bigint NOT NULL)

	INSERT INTO @FirmsToUpdate
	SELECT F.Id, F.OrganizationUnitId, F.DgppId FROM @buildings buildings
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
	INNER JOIN @FirmsToUpdate dto ON F.DgppId = dto.DgppId
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

            sp.Alter();
        }
    }
}
