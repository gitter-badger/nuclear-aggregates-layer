using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
   [Migration(7088, "Ограничение кол-ва реплицируемых объектов")]
    public sealed class Migration7088 : TransactedMigration
    {
        #region Текст запроса
       private const string Query = @"ALTER PROCEDURE [Integration].[ReplicateObjectsAfterImportCards]
AS
BEGIN

SET XACT_ABORT ON;

BEGIN TRY;

-- Удалаяем дубли фирм, нуждающихся в обработке
WITH CTE AS (
	SELECT FirmId, ROW_NUMBER() OVER(PARTITION BY FirmId ORDER BY FirmId, CreatedOn DESC) rnk
	FROM Integration.FirmsForPostIntegrationActivities
	 )
DELETE FROM CTE
WHERE rnk > 1;

--Репликация объектов
BEGIN TRAN
DECLARE @FirmIds Shared.Int32IdsTableType
DECLARE @AddressIds Shared.Int32IdsTableType

INSERT INTO @FirmIds SELECT DISTINCT TOP 1000 FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ReplicateObjects = 0
INSERT INTO @AddressIds SELECT Id FROM BusinessDirectory.FirmAddresses WHERE FirmId IN (SELECT Id FROM @FirmIds)

EXEC BusinessDirectory.ReplicateFirms @FirmIds
EXEC BusinessDirectory.ReplicateFirmAddresses @AddressIds

UPDATE Integration.FirmsForPostIntegrationActivities SET ReplicateObjects = 1 WHERE FirmId In (SELECT Id FROM @FirmIds)
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1
COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}


