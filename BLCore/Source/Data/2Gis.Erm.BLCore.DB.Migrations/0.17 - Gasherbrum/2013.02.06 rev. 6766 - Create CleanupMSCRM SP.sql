CREATE PROCEDURE [Shared].[CleanupMSCRM]
	-- Add the parameters for the stored procedure here
	@logSizeInDays int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

DECLARE @now datetime
SELECT @now = GETDATE()

DECLARE @DeleteRowCount int
SELECT @DeleteRowCount = 2000
DECLARE @DeletedAsyncRowsTable TABLE (AsyncOperationId uniqueidentifier NOT NULL PRIMARY KEY)

DECLARE @continue int, @rowCount int
select @continue = 1
WHILE (@continue = 1)
BEGIN      
BEGIN TRAN
INSERT INTO @DeletedAsyncRowsTable(AsyncOperationId)
      SELECT TOP (@DeleteRowCount) AsyncOperationId FROM [DoubleGis_MSCRM].dbo.AsyncOperationBase
      WHERE DATEDIFF(day, CreatedOn, @now) > @logSizeInDays
       
      SELECT @rowCount = 0
      SELECT @rowCount = count(*) FROM @DeletedAsyncRowsTable
      SELECT @continue = CASE WHEN @rowCount <= 0 THEN 0 ELSE 1 END
        IF (@continue = 1)        
        BEGIN
            DELETE [DoubleGis_MSCRM].dbo.WorkflowLogBase FROM [DoubleGis_MSCRM].dbo.WorkflowLogBase W, @DeletedAsyncRowsTable d
            WHERE W.AsyncOperationId = d.AsyncOperationId             
           
            DELETE [DoubleGis_MSCRM].dbo.AsyncOperationBase FROM [DoubleGis_MSCRM].dbo.AsyncOperationBase A, @DeletedAsyncRowsTable d
            WHERE A.AsyncOperationId = d.AsyncOperationId             
            DELETE @DeletedAsyncRowsTable      
        END
COMMIT TRAN
END

END
