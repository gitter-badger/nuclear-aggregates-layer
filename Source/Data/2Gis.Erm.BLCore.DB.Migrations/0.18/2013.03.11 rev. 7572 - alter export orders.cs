using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7572, "Alter экспорта заказов в связи с отложенной репликацией")]
    public sealed class Migration7572 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"
IF OBJECT_ID('[Integration].[ReplicateObjectsAfterImportCards]') IS NOT NULL 
DROP PROCEDURE [Integration].[ReplicateObjectsAfterImportCards]
GO


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
GO

ALTER VIEW [Security].[vwUsersDescendants]
AS
	WITH CTE (AncestorId, DescendantId, [Level]) AS
	(
		SELECT ParentId, Id, 1 AS [Level] FROM [Security].[Users] WHERE ParentId IS NOT NULL AND IsActive = 1 AND IsDeleted = 0
		
		UNION ALL
		
		SELECT CTE.AncestorId, U.Id, [Level] + 1 AS [Level] FROM CTE INNER JOIN [Security].[Users] AS U ON U.ParentId = CTE.DescendantId AND U.IsActive = 1 AND U.IsDeleted = 0
	)

	SELECT 
		ISNULL( ROW_NUMBER() OVER (ORDER BY AncestorId, DescendantId), 0 ) AS Id, AncestorId, DescendantId, [Level]
	FROM CTE

GO
";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
