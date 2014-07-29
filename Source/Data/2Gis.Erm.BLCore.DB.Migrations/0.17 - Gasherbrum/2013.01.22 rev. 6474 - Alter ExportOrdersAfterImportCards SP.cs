using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6474, "Изменение функции экспорта заказов после импорта карточек")]
    public class Migration6474 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            #region Текст запроса
            const string Query = @"
DROP PROCEDURE [Integration].[PostIntegrationActivitiesWithFirms]
GO

CREATE PROCEDURE [Integration].[ExportOrdersAfterImportCards]
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

--Выгружаем заказы, связанные с этими фирмами
BEGIN TRAN
DECLARE @ArchiveWorkflowStepId int
SET @ArchiveWorkflowStepId = 6
DECLARE @FakeField int
UPDATE Billing.Orders 
SET @FakeField = 1
WHERE WorkflowStepId != @ArchiveWorkflowStepId AND FirmId in (SELECT FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 0)
UPDATE Integration.FirmsForPostIntegrationActivities SET ExportOrders = 1
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1
COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END
GO";
            #endregion

            context.Connection.ExecuteNonQuery(Query);
        }
    }
}