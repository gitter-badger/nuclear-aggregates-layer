-- =============================================
-- Author:		<Grishnov E.N.>
-- Create date: <13/07/2010>
-- Description:	<Write Log Info>
-- =============================================
CREATE PROCEDURE [dbo].[WriteLogMessage] 
@SeanceCode		char(36),
@SessionID		NVARCHAR(50),
@UserName		NVARCHAR(100),
@UserIP			NVARCHAR(50),
@UserBrowser	NVARCHAR(100),
@MessageDate	datetime2,
@MessageLevel		char(5),
@MessageText		NVARCHAR(MAX),
@ExceptionData		NVARCHAR(MAX),
@MethodName			NVARCHAR(250),
@InputParameters	NVARCHAR(1024)
AS
BEGIN
SET NOCOUNT ON;
Declare @UserDataID		int
Declare @UserDataHash	NVARCHAR(50)
Declare @Str			NVARCHAR(400)

---------------UserData Check
SET @Str = ISNULL(@SeanceCode,'')+ISNULL(@SessionID,'')+ISNULL(@UserIP,'')+ISNULL(@UserName,'')+ISNULL(@UserBrowser,'')+'S0lt68~'
SET @UserDataHash=HASHBYTES('SHA1', @str)

SELECT @UserDataId=Id from [dbo].[UserData] where HashCode=@UserDataHash
IF(@UserDataId is NULL)
	Begin
		--IF((@SessionID is not null) AND (@SessionID='(null)')) 
		--	SET @SessionID=NULL
			
		--IF((@UserIP is not null) AND (@UserIP='(null)')) 
		--	SET @UserIP=NULL
				
		--IF((@UserName is not null) AND (@UserName='(null)'))
		--	SET @UserName=NULL
			
		--IF((@UserBrowser is not null) AND (@UserBrowser='(null)'))
		--	SET @UserBrowser=NULL
		
		INSERT INTO [dbo].[UserData](SeanceID, SessionID, UserIP, UserName, UserBrowser, HashCode)
			VALUES(CAST(@SeanceCode as uniqueidentifier), @SessionID, @UserIP, @UserName, @UserBrowser, @UserDataHash)
		SET @UserDataId = SCOPE_IDENTITY()
	End

---------------Write message
INSERT INTO [dbo].[LogJournal](UserDataID, MessageDate, MessageLevel, MessageText, ExceptionData, MethodName, InputParameters)
	VALUES (@UserDataID, @MessageDate, @MessageLevel, @MessageText, @ExceptionData, @MethodName, @InputParameters)

END