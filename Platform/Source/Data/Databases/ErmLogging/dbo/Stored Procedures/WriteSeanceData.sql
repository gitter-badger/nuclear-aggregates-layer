CREATE Procedure [dbo].[WriteSeanceData]
	@SeanceCode  char(36), 
	@ModuleName  NVARCHAR(50),
	@MessageDate datetime2, 
	@ConfigFile  NVARCHAR(max)
AS
SET NOCOUNT ON;
Declare @ModuleId tinyint
Select @ModuleId=Id from Modules where Name=@ModuleName
IF(@ModuleId is null)
Begin
	INSERT INTO Modules(Name) VALUES (@ModuleName)
	SET @ModuleId = SCOPE_IDENTITY()
End

INSERT INTO [dbo].[Seances]([Id], [ModuleId], [MessageDate], [ConfigFile]) 
	VALUES(CAST(@SeanceCode as uniqueidentifier), @ModuleId, @MessageDate, @ConfigFile)