using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13798, "Меняем способ обновления территории фирмы", "y.baranihin")]
    public sealed class Migration13798 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddGetFirmTerritoryFunction(context);
            AlterImportFirmFromXml(context);
            AlterUpdateBuildings(context);
        }

        private void AddGetFirmTerritoryFunction(IMigrationContext context)
        {
            #region текст хранимки
            var query = @"CREATE PROCEDURE [Shared].[GetFirmTerritories](
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
        LEFT JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = IsNull(FA.TerritoryId, B.TerritoryId)
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 1 AND FA.IsDeleted = 0 AND FA.ClosedForAscertainment = 0
        GROUP BY FirmId 

	-- теперь не побрезгуем скрытыми до выяснения
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        LEFT JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = IsNull(FA.TerritoryId, B.TerritoryId)
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 1 AND FA.IsDeleted = 0 AND FA.ClosedForAscertainment = 1
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

	-- ок, теперь можно и на скрытые посмотреть
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        LEFT JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = IsNull(FA.TerritoryId, B.TerritoryId)
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsActive = 0 AND FA.IsDeleted = 0
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

	-- доходим до отчаяния - смотрим даже удаленные адреса
		INSERT INTO @FirmTerritoriesInfo
        SELECT MIN(FA.SortingPosition), FirmId, NULL FROM 
		 BusinessDirectory.Firms F
		 INNER JOIN  @FirmIds ids ON F.Id = ids.id
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = F.Id
        LEFT JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
        INNER JOIN BusinessDirectory.Territories T ON T.Id = IsNull(FA.TerritoryId, B.TerritoryId)
            WHERE T.OrganizationUnitId = F.OrganizationUnitId  AND FA.IsDeleted = 1
			AND F.Id NOT IN (SELECT FirmId FROM @FirmTerritoriesInfo)
        GROUP BY FirmId 

		UPDATE FI SET TerritoryId = T.Id FROM @FirmTerritoriesInfo FI
		INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.FirmId = FI.FirmId AND FA.SortingPosition = FI.SortingPosition
		INNER JOIN Integration.Buildings B ON FA.BuildingCode = B.Code AND B.IsDeleted = 0
		INNER JOIN BusinessDirectory.Territories T ON T.Id = IsNull(FA.TerritoryId, B.TerritoryId)

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
";
            #endregion

            context.Connection.ExecuteNonQuery(query);
        }

        private void AlterImportFirmFromXml(IMigrationContext context)
        {
            #region текст хранимки

            var query = @"-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   24.06.2013, a.rechkalov: замена int -> bigint
--   10.09.2013, y.baranihin: dgppid -> id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
--   14.11.2013, a.tukaev: поддержка импорта flowCardsForERM 
--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
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
        FROM OPENXML (@xmlHandle, N'Root/Firm', 0) WITH (Code BIGINT, BranchCode INT, Name NVARCHAR(250), PromotionalName NVARCHAR(250), IsArchived bit, IsHidden bit) XmlFirmDtos

        -- @Cards
        DECLARE @Cards TABLE (InsertOrder INT IDENTITY(1, 1) NOT NULL, FirmCode BIGINT NOT NULL, Code BIGINT NOT NULL, SortingPosition INT NOT NULL)
        INSERT INTO @Cards
        SELECT FirmCode, Code, 0 FROM OPENXML (@xmlHandle, N'Root/Firm/Card', 1) WITH (FirmCode BIGINT N'../@Code', Code BIGINT N'@Code')

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
               --set @MessageOrganizationUnitInvalidDgppId = concat(N'Подразделение с кодом ДГПП ', @InvalidDgppId, N' не найдено в системе')
               set @MessageOrganizationUnitInvalidDgppId = N'Подразделение с кодом ДГПП ' + CONVERT(nvarchar(32), @InvalidDgppId) + N' не найдено в системе'
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

		 -- заполняем таблицу новых территорий фирм
        DECLARE @FirmTerritories TABLE (FirmId bigint NOT NULL, TerritoryId bigint NOT NULL)
		DECLARE @FirmIds Shared.Int64IdsTableType
		
		INSERT INTO @FirmIds
        SELECT DISTINCT DgppId FROM @FirmDtos

		INSERT INTO @FirmTerritories EXEC Shared.GetFirmTerritories @FirmIds = @FirmIds, @RegionalTerritoryLocalName = @RegionalTerritoryLocalName

		UPDATE F
        SET F.TerritoryId = FT.TerritoryId
        FROM BusinessDirectory.Firms F
        INNER JOIN @FirmTerritories FT ON F.Id = FT.FirmId

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
    END CATCH
";

            #endregion

            context.Connection.ExecuteNonQuery(query);
        }

        private void AlterUpdateBuildings(IMigrationContext context)
        {
            #region текст хранимки

            var query = @"-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
--   10.09.2013, y.baranihin: dgppid->id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
ALTER PROCEDURE [Integration].[UpdateBuildings]
       @buildingsXml [xml],
	   @RegionalTerritoryLocalName nvarchar(255)
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
    SET F.TerritoryId = FT.TerritoryId
    FROM BusinessDirectory.Firms F
    INNER JOIN @FirmTerritories FT ON F.Id = FT.FirmId

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
END CATCH
";
            #endregion

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
