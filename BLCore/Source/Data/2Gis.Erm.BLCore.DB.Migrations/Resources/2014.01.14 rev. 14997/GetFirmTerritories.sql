-- changes
--   12.12.2013, a.tukaev: fix ERM-2651 
--   14.01.2014, a.tukaev: ...и его откат
ALTER PROCEDURE [Shared].[GetFirmTerritories](
	@FirmIds Shared.Int64IdsTableType READONLY,
	@RegionalTerritoryLocalName nvarchar(255)
)
AS
BEGIN
	-- определяем первый активный адрес для фирмы, чтобы далее обновить у нее территорию 
	-- сначала смотрим только активные адреса
        DECLARE @FirmTerritoriesInfo TABLE(SortingPosition int NOT NULL, FirmId bigint NOT NULL, TerritoryId bigint NULL)
        INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = B.TerritoryId
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 1 AND FA.IsDeleted = 0 AND FA.ClosedForAscertainment = 0
        GROUP BY FirmId 

	-- теперь не побрезгуем скрытыми до выяснения
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = B.TerritoryId
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 1 AND FA.IsDeleted = 0 AND FA.ClosedForAscertainment = 1
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

	-- ок, теперь можно и на скрытые посмотреть
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = B.TerritoryId
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 0 AND FA.IsDeleted = 0
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

	-- доходим до отчаяния - смотрим даже удаленные адреса
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = B.TerritoryId
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsDeleted = 1
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

		UPDATE FI SET TerritoryId = T.Id FROM @FirmTerritoriesInfo FI
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = FI.FirmId AND FA.SortingPosition = FI.SortingPosition
		INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
		INNER JOIN BusinessDirectory.Territories T ON T.Id = B.TerritoryId

	-- Оставшимся фирмам выставим региональную территорию.
		INSERT INTO @FirmTerritoriesInfo
        SELECT 0, F.Id, min(T.Id)  FROM 
		 BusinessDirectory.Firms F
		INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.Territories T ON T.OrganizationUnitId = F.OrganizationUnitId        
		AND T.Name like N'%' + @RegionalTerritoryLocalName + N'%' AND T.IsActive = 1
            WHERE F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY F.Id 

	-- Все остальные фирмы территорию не поменяют
	SELECT FirmId, TerritoryId FROM @FirmTerritoriesInfo
END
