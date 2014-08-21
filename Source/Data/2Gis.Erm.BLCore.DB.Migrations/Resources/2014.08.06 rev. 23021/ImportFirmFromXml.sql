-- changes
--   5.06.2013, a.rechkalov: добавил параметр RegionalTerritoryLocalName
--   24.06.2013, a.rechkalov: замена int -> bigint
--   10.09.2013, y.baranihin: dgppid -> id
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
--   14.11.2013, a.tukaev: поддержка импорта flowCardsForERM 
--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
--   20.05.2014, i.maslennikov: поддержка асинхронной репликации
--   06.08.2014, a.tukaev: fix ERM-4693
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

		DECLARE @UpdatedAddresses TABLE (Id bigint NOT NULL)
        -- вяжем адреса с фирмой и проставляем sorting position
        UPDATE FA
        SET	FirmId = F.Id,
            SortingPosition = Cards.SortingPosition,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETUTCDATE()
		OUTPUT inserted.Id INTO @UpdatedAddresses
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

		-- отчет для вызывающего кода об измененных сущностях
		DECLARE @EntityName_Firm int = 146,
				@EntityName_FirmAddress int = 164,
				@EntityName_Territory int = 191

		DECLARE @ChangeType_Created int = 1,
				@ChangeType_Deleted int = 2,
				@ChangeType_Updated int = 3

		DECLARE @ChangedEntities TABLE (Id bigint NOT NULL, EntityName int NOT NULL, ChangeType int NOT NULL)
		INSERT INTO @ChangedEntities SELECT f.Id as Id, @EntityName_Firm as EntityName, @ChangeType_Updated as ChangeType FROM @FirmIds f
		INSERT INTO @ChangedEntities SELECT fad.Id as Id, @EntityName_FirmAddress as EntityName, @ChangeType_Deleted as ChangeType FROM @DeletedAddresses fad
		INSERT INTO @ChangedEntities SELECT fau.Id as Id, @EntityName_FirmAddress as EntityName, @ChangeType_Updated as ChangeType FROM @UpdatedAddresses fau

		SELECT * FROM @ChangedEntities

        COMMIT TRAN

    END TRY
    BEGIN CATCH
        IF (XACT_STATE() != 0)
            ROLLBACK TRAN

        DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
        SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH

