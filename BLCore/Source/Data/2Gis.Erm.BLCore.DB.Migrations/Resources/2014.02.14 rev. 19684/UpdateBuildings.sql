-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
--   10.09.2013, y.baranihin: dgppid->id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
--   29.01.2014, y.baranihin: при изменении территории у фирмы будем проставлять дату изменения
--   30.01.2014, y.baranihin: включаем репликацию клиентов
--   14.02.2014, a.tukaev: добавялем параметр @enableReplication
--   18.04.2014, a.tuakev: ERM-2904 fix
ALTER PROCEDURE [Integration].[UpdateBuildings]
       @buildingsXml [xml],
	   @RegionalTerritoryLocalName nvarchar(255),
	   @EnableReplication [bit] = 1
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
	FROM OPENXML(@docHandle, N'/buildings/building', 1) WITH (Code BIGINT, SaleTerritoryCode BIGINT, IsDeleted BIT)

	IF (EXISTS(SELECT 1 FROM @xmlBuildings xmlBuildings WHERE SaleTerritoryCode NOT IN (SELECT Id FROM BusinessDirectory.Territories) ))
	BEGIN
		RAISERROR (N'Cant find SaleTerritoryCode in BusinessDirectory.Territories table', 16, 2) WITH SETERROR
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
	-- заполняем таблицу новых территорий фирм
    DECLARE @FirmTerritories TABLE (FirmId bigint NOT NULL, TerritoryId bigint NOT NULL)
	DECLARE @FirmIds Shared.Int64IdsTableType
		
	INSERT INTO @FirmIds
    SELECT DISTINCT F.Id FROM @buildings buildings
	inner join BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = buildings.Code
	inner join BusinessDirectory.Firms F ON F.Id = FA.FirmId

	INSERT INTO @FirmTerritories EXEC Shared.GetFirmTerritories @FirmIds = @FirmIds, @RegionalTerritoryLocalName = @RegionalTerritoryLocalName

	UPDATE F
    SET F.TerritoryId = FT.TerritoryId, ModifiedOn = GETUTCDATE(), ModifiedBy = 1846
    FROM BusinessDirectory.Firms F
    INNER JOIN @FirmTerritories FT ON F.Id = FT.FirmId

    -- update client territory
    DECLARE @ClientIdTable TABLE (Id bigint NOT NULL)

	UPDATE C
	SET C.TerritoryId = buildings.TerritoryId, ModifiedOn = GETUTCDATE(), ModifiedBy = 1846
	OUTPUT inserted.Id INTO @ClientIdTable
	FROM @buildings buildings
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = buildings.Code
	INNER JOIN BusinessDirectory.Firms F ON F.Id = FA.FirmId
	INNER JOIN Billing.Clients C ON C.Id = F.ClientId
	WHERE C.MainFirmId IS NULL OR C.MainFirmId = F.Id

	--replicate clients
	IF (@EnableReplication = 1)
	BEGIN
		DECLARE @currentId bigint
		SELECT @currentId = MIN(Id) FROM @ClientIdTable

		WHILE @currentId IS NOT NULL
		BEGIN
			EXEC Billing.ReplicateClient @Id = @currentId
			SELECT @currentId = MIN(Id) FROM @ClientIdTable WHERE Id > @currentId
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
END CATCH
