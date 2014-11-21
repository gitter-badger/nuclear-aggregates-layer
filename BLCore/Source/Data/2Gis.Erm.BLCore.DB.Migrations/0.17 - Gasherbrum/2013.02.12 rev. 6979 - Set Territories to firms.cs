using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6979, "Актуализируем территории фирм")]
    public sealed class Migration6979 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"-- определим фирмы, у которых нужно поправить территорию. 
DECLARE @FirmsToFix TABLE (FirmId int not null, OrganizationUniId int not null, IsUpdated bit) 
INSERT INTO @FirmsToFix
SELECT DISTINCT f.Id, f.OrganizationUnitId, 0 FROM BusinessDirectory.Firms f INNER JOIN BusinessDirectory.Territories t ON f.TerritoryId = t.Id WHERE f.OrganizationUnitId <> t.OrganizationUnitId

-- определяем подходящий адрес для фирмы, чтобы далее обновить у нее территорию 
	DECLARE @FirstActiveAdressForFirm TABLE(SortingPosition int NOT NULL, FirmId int NOT NULL)
	INSERT INTO @FirstActiveAdressForFirm
	SELECT min(FA.SortingPosition), FirmId 
	FROM BusinessDirectory.FirmAddresses FA 
	inner join BusinessDirectory.Firms f on fa.FirmId = f.Id 
	inner join Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
	inner join BusinessDirectory.Territories t on t.id = b.TerritoryId 
        WHERE 
		t.OrganizationUnitId = f.OrganizationUnitId 
		AND FA.IsActive = 1 
		AND FA.IsDeleted = 0
		AND FA.ClosedForAscertainment = 0
		AND F.Id in (SELECT FirmId FROM @FirmsToFix)
    GROUP BY FirmId 

	DECLARE @FirmsFixed TABLE (FirmId int not null) 

	UPDATE F
	SET F.TerritoryId = B.TerritoryId	
	OUTPUT inserted.Id into @FirmsFixed
	FROM BusinessDirectory.Firms F 
	INNER JOIN @FirstActiveAdressForFirm AA ON AA.FirmId = F.Id
	INNER JOIN @FirmsToFix FF ON AA.FirmId = FF.FirmId
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
	INNER JOIN Integration.Buildings B ON B.Code = FA.BuildingCode
	WHERE FA.SortingPosition = AA.SortingPosition
	
	UPDATE @FirmsToFix SET IsUpdated = 1 WHERE FirmId in (SELECT FirmId FROM @FirmsFixed)

	-- для фирм у которых территория не обновилась проставим региональную
DECLARE @TempTbl TABLE
		(
			TerritoryId int, 
			OrganizationUnitId int
		)

INSERT INTO @TempTbl SELECT 
			Id, OrganizationUnitId 
			FROM BusinessDirectory.Territories WHERE OrganizationUnitId IN (SELECT DISTINCT OrganizationUnitId FROM @FirmsToFix WHERE IsUpdated = 0)
			AND IsActive = 1 AND Name like '%Региональная территория%'


UPDATE F
	SET F.TerritoryId = AA.TerritoryId	
	FROM BusinessDirectory.Firms F 
	INNER JOIN @FirmsToFix FF ON F.Id = FF.FirmId
	INNER JOIN @TempTbl AA ON AA.OrganizationUnitId = FF.OrganizationUniId
	WHERE FF.IsUpdated = 0
";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
