-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Shared].[CleanupERM]
	-- Add the parameters for the stored procedure here
	@logSizeInDays int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF @logSizeInDays IS NULL
RAISERROR(N'The value for @logSizeInDays should not be null', 15, 1)

DECLARE @dateLowerBound datetime
SELECT @dateLowerBound = DATEADD(DAY, -1 * @logSizeInDays, GETDATE())

-- Локальные сообщения и связанные файлы.
BEGIN TRAN

DECLARE @localMessagesToDelete TABLE ( Id bigint, FileId bigint );
INSERT @localMessagesToDelete SELECT Id, FileId FROM Shared.LocalMessages WHERE EventDate < @dateLowerBound

DELETE FROM Shared.LocalMessages WHERE Id IN (SELECT Id FROM @localMessagesToDelete)
DELETE FROM Shared.FileBinaries WHERE Id IN (SELECT FileId FROM @localMessagesToDelete)
DELETE FROM Shared.Files WHERE Id IN (SELECT FileId FROM @localMessagesToDelete)

COMMIT TRAN

DELETE @localMessagesToDelete

-- Операции и связанные файлы.
BEGIN TRAN

DECLARE @operationsToDelete TABLE ( Id bigint, LogFileId bigint );
INSERT @operationsToDelete SELECT Id, LogFileId FROM Shared.Operations o WHERE StartTime < @dateLowerBound

DELETE FROM Shared.Operations WHERE Id IN(SELECT Id FROM @operationsToDelete)
DELETE FROM Shared.FileBinaries WHERE Id IN (SELECT LogFileId FROM @operationsToDelete)
DELETE FROM Shared.Files WHERE Id IN (SELECT LogFileId FROM @operationsToDelete)

COMMIT TRAN

DELETE @operationsToDelete

END
