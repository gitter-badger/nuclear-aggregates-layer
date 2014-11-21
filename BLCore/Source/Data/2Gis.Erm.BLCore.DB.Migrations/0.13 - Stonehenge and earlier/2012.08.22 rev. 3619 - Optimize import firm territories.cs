using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(3619, "Оптимизация загрузки территорий из шины интеграции")]
	public sealed class Migration3619 : TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
			// удалить FirmAddressBuildings, вместо неё будет колонка в FirmAddresses
			AddBuildingCodeColumnToFirmAddressTable(context);
			UpdateFirmAddressBuildings(context);
			AddFirmAddressAndBuildingsForeignKey(context);
			UpdateBuildingsToRegionalTerritories(context);
			AlterBuildingsTerritoryIdNotNullable(context);
			AddBuildingsIsDeletedColumn(context);

			// сделать alter для хранимых процедур
			DropFirmAddressBuildingsSp(context);
			AlterGetTemporaryTerritorySp(context);
			AlterUpdateBuildingsSp(context);
		}

		private static void AlterUpdateBuildingsSp(IMigrationContext context)
		{
			var updateBuildingsSp = context.Database.StoredProcedures["UpdateBuildings", ErmSchemas.Integration];

			updateBuildingsSp.TextBody = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRAN
	
	DECLARE @docHandle INT

	EXEC sp_xml_preparedocument @docHandle OUTPUT, @buildingsXml

	DECLARE @xmlBuildings TABLE (Code BIGINT NOT NULL, SaleTerritoryCode BIGINT NOT NULL, IsDeleted BIT NOT NULL)
	INSERT INTO @xmlBuildings
	SELECT
	Code,
	SaleTerritoryCode,
	COALESCE(IsDeleted, 0) AS IsDeleted -- xsd default value
	FROM OPENXML(@docHandle, '/buildings/building', 1) WITH (Code BIGINT, SaleTerritoryCode BIGINT, IsDeleted BIT)

	IF (EXISTS(SELECT 1 FROM @xmlBuildings xmlBuildings WHERE SaleTerritoryCode NOT IN (SELECT DgppId FROM BusinessDirectory.Territories) ))
	BEGIN
		RAISERROR ('Cant find SaleTerritoryCode in BusinessDirectory.Territories table', 16, 2) WITH SETERROR
		RETURN
	END

	DECLARE @buildings TABLE(Code BIGINT NOT NULL, TerritoryId INT NOT NULL, IsDeleted BIT NOT NULL)
	INSERT INTO @buildings
	SELECT
	xmlBuildings.Code,
	T.Id,
	xmlBuildings.IsDeleted
	FROM @xmlBuildings xmlBuildings
	INNER JOIN BusinessDirectory.Territories T ON T.DgppId = xmlBuildings.SaleTerritoryCode

	EXEC sp_xml_removedocument @docHandle
	
	-- делаем update существующих и insert отсутствующих записей
	MERGE Integration.Buildings B
	USING @buildings buildings
	ON buildings.Code = B.Code
	WHEN MATCHED THEN
		UPDATE SET B.TerritoryId = buildings.TerritoryId,
					B.IsDeleted = buildings.IsDeleted
	WHEN NOT MATCHED THEN
		INSERT (Code, TerritoryId, IsDeleted) VALUES (buildings.Code, buildings.TerritoryId, buildings.IsDeleted);


    -- update firm territory
    DECLARE @FrimIdTable TABLE (Id INT NOT NULL)

	UPDATE F
	SET F.TerritoryId = buildings.TerritoryId
	OUTPUT inserted.Id INTO @FrimIdTable
	FROM @buildings buildings
	INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = buildings.Code
	INNER JOIN BusinessDirectory.Firms F ON F.Id = FA.FirmId

	-- replicate firms
	DECLARE @currentId INT
	SELECT @currentId = MIN(Id) FROM @FrimIdTable

	WHILE @currentId IS NOT NULL
	BEGIN
		EXEC BusinessDirectory.ReplicateFirm @Id = @currentId
		SELECT @currentId = MIN(Id) FROM @FrimIdTable WHERE Id > @currentId
	END

    -- update client territory
    DECLARE @ClientIdTable TABLE (Id INT NOT NULL)

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

			updateBuildingsSp.Alter();
		}

		private static void AlterGetTemporaryTerritorySp(IMigrationContext context)
		{
			var getTemporaryTerritorySp = context.Database.StoredProcedures["GetTemporaryTerritory", ErmSchemas.Integration];

			getTemporaryTerritorySp.TextBody = @"
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

BEGIN TRY

	IF (@OrganizationUnitId IS NULL)
	BEGIN
		SELECT NULL
		RETURN
	END

	-- плохо использовать понятие временной территории, потом надо избавиться от этого
	DECLARE @TerritoryId INT
	SELECT @TerritoryId = Id FROM BusinessDirectory.Territories WHERE OrganizationUnitId = @OrganizationUnitId AND IsActive = 1 AND IsDeleted = 0 AND Name like '%Региональная территория%'

	IF (@TerritoryId IS NULL)
	BEGIN
		BEGIN TRAN

		DECLARE @TemporaryTerritoryName NVARCHAR(255)
		SELECT @TemporaryTerritoryName = Name + '. Региональная территория' FROM Billing.OrganizationUnits WHERE Id = @OrganizationUnitId

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

			getTemporaryTerritorySp.Alter();
		}

		private static void DropFirmAddressBuildingsSp(IMigrationContext context)
		{
			var storedProc = context.Database.StoredProcedures["UpdateFirmAddressBuildings", ErmSchemas.Integration];
			if (storedProc == null)
				return;

			storedProc.Drop();
		}

		private static void AddBuildingsIsDeletedColumn(IMigrationContext context)
		{
			var table = context.Database.Tables["Buildings", ErmSchemas.Integration];

			var isDeletedColumn = table.Columns["IsDeleted"];
			if (isDeletedColumn != null)
				return;

			isDeletedColumn = new Column(table, "IsDeleted", DataType.Bit) {Nullable = false};
			isDeletedColumn.AddDefaultConstraint("DF_Buildings_IsDeleted").Text = "0";

			isDeletedColumn.Create();
		}

		private static void AlterBuildingsTerritoryIdNotNullable(IMigrationContext context)
		{
			var table = context.Database.Tables["Buildings", ErmSchemas.Integration];

			DropIndex(table);

			var territoryIdColumn = table.Columns["TerritoryId"];
			if (!territoryIdColumn.Nullable)
				return;

			context.Database.ExecuteNonQuery(@"
			DELETE FROM Integration.Buildings WHERE TerritoryId IS NULL
			");

			territoryIdColumn.Nullable = false;
			territoryIdColumn.Alter();
		}

		private static void DropIndex(TableViewTableTypeBase table)
		{
			var index = table.Indexes["IX_BuildingTerritories"];
			if (index == null)
				return;

			index.Drop();
		}

		private static void UpdateBuildingsToRegionalTerritories(IMigrationContext context)
		{
			context.Database.ExecuteNonQuery(@"
			UPDATE B
			SET B.TerritoryId = T.Id
			FROM Integration.Buildings B
			INNER JOIN BusinessDirectory.FirmAddresses FA ON FA.BuildingCode = B.Code
			INNER JOIN BusinessDirectory.Firms F ON F.Id = FA.FirmId
			INNER JOIN Billing.OrganizationUnits O ON O.Id = F.OrganizationUnitId
			INNER JOIN BusinessDirectory.Territories T ON T.OrganizationUnitId = O.Id
			WHERE
			B.TerritoryId IS NULL
			AND
			T.IsActive = 1 AND (T.Name like '%регион%' OR T.Name like '%времен%')");
		}

		private static void AddFirmAddressAndBuildingsForeignKey(IMigrationContext context)
		{
			const string foreignKeyName = "FK_FirmAddresses_Buildings";

			var table = context.Database.Tables["FirmAddresses", ErmSchemas.BusinessDirectory];

			var foreignKey = table.ForeignKeys[foreignKeyName];
			if (foreignKey != null)
				return;

			// create foreign key
			foreignKey = new ForeignKey(table, foreignKeyName);
			foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "BuildingCode", "Code"));
			foreignKey.ReferencedTable = "Buildings";
			foreignKey.ReferencedTableSchema = ErmSchemas.Integration;
			foreignKey.Create();
		}

		private static void UpdateFirmAddressBuildings(IMigrationContext context)
		{
			var table = context.Database.Tables["FirmAddressBuildings", ErmSchemas.Integration];
			if (table == null)
				return;

			context.Database.ExecuteNonQuery(@"
			UPDATE FA
			SET FA.BuildingCode = FAB.BuildingCode
			FROM BusinessDirectory.FirmAddresses FA
			INNER JOIN
			Integration.FirmAddressBuildings FAB ON FAB.FirmAddressId = FA.Id
			WHERE FA.BuildingCode <> FAB.BuildingCode");

			table.Drop();
		}

		private static void AddBuildingCodeColumnToFirmAddressTable(IMigrationContext context)
		{
			const string buildingCode = "BuildingCode";

			var table = context.Database.Tables["FirmAddresses", ErmSchemas.BusinessDirectory];

			var column = table.Columns[buildingCode];
			if (column != null)
				return;

			column = new Column(table, buildingCode, DataType.BigInt) {Nullable = true};
			column.Create();
		}
	}
}