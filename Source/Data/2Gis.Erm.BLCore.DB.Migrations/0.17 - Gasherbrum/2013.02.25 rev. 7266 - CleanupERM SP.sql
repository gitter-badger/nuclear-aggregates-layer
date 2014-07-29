SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [Shared].[CleanupERM]
	-- Add the parameters for the stored procedure here
	@logSizeInDays int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF @logSizeInDays IS NULL
RAISERROR('The value for @logSizeInDays should not be null', 15, 1)

DECLARE @dateLowerBound datetime
SELECT @dateLowerBound = DATEADD(DAY, -1 * @logSizeInDays, GETDATE())

-- Логи ERM
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

-- Локальные сообщения и связанные файлы.
BEGIN TRAN

DECLARE @localMessagesToDelete TABLE ( Id int, FileId int );
INSERT @localMessagesToDelete SELECT Id, FileId FROM Shared.LocalMessages WHERE EventDate < @dateLowerBound

DELETE FROM Shared.LocalMessages WHERE Id IN (SELECT Id FROM @localMessagesToDelete)
DELETE FROM Shared.FileBinaries WHERE Id IN (SELECT FileId FROM @localMessagesToDelete)
DELETE FROM Shared.Files WHERE Id IN (SELECT FileId FROM @localMessagesToDelete)

COMMIT TRAN

DELETE @localMessagesToDelete

-- Операции и связанные файлы.
BEGIN TRAN

DECLARE @operationsToDelete TABLE ( Id int, LogFileId int );
INSERT @operationsToDelete SELECT Id, LogFileId FROM Shared.Operations o WHERE StartTime < @dateLowerBound

DELETE FROM Shared.Operations WHERE Id IN(SELECT Id FROM @operationsToDelete)
DELETE FROM Shared.FileBinaries WHERE Id IN (SELECT LogFileId FROM @operationsToDelete)
DELETE FROM Shared.Files WHERE Id IN (SELECT LogFileId FROM @operationsToDelete)

COMMIT TRAN

DELETE @operationsToDelete

END