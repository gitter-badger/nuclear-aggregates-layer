ALTER PROCEDURE [Integration].[ExportOrdersAfterImportCards]
AS

BEGIN
SET XACT_ABORT ON;

BEGIN TRY;

--Выгружаем заказы, связанные с этими фирмами
BEGIN TRAN

DECLARE @ArchiveWorkflowStepId INT = 6
DECLARE @FakeField INT
UPDATE Billing.Orders 
SET @FakeField = 1
WHERE
WorkflowStepId != @ArchiveWorkflowStepId
AND
FirmId IN (SELECT FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 0)

UPDATE Integration.FirmsForPostIntegrationActivities SET ExportOrders = 1
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND SyncFirmAddresses = 1

COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END

DROP PROCEDURE [Integration].[ReplicateObjectsAfterImportCards]
GO 
