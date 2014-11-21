using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3534, "Alter UpdateFirmAddressBuildings stored procedure")]
    public sealed class Migration3534 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
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
	
	-- insert new buildings
	INSERT INTO Integration.Buildings (Code)
	SELECT BuildingCode FROM @links L
	WHERE L.BuildingCode IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Integration.Buildings WHERE Code = L.BuildingCode)

	-- update existing relations
	UPDATE FAB
	SET FAB.BuildingCode = L.BuildingCode
	FROM Integration.FirmAddressBuildings FAB
	INNER JOIN
	@links L ON L.FirmAddressId = FAB.FirmAddressId
	
	-- insert new relations
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
    }
}