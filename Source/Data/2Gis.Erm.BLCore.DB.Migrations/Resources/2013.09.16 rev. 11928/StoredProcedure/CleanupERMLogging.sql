-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Shared].[CleanupERMLogging]
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

-- Логи ERM
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
BEGIN TRAN

DECLARE @logEntriesToDelete TABLE ( SeanceId uniqueidentifier, UserDataId int );

INSERT @logEntriesToDelete SELECT DISTINCT s.Id, ud.Id 
FROM [ErmLogging].dbo.Seances s LEFT OUTER JOIN [ErmLogging].dbo.UserData ud ON ud.SeanceID = s.Id
WHERE s.MessageDate < @dateLowerBound

DELETE FROM [ErmLogging].dbo.LogJournal WHERE UserDataID IN (SELECT UserDataID FROM @logEntriesToDelete)
DELETE FROM [ErmLogging].dbo.UserData WHERE Id IN (SELECT UserDataID FROM @logEntriesToDelete)
DELETE FROM [ErmLogging].dbo.Seances WHERE Id IN (SELECT SeanceId FROM @logEntriesToDelete)

COMMIT TRAN

DELETE @logEntriesToDelete

END
