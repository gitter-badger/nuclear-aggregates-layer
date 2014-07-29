using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7832, "Изменение параметров хранимок DepartmentDelete, DepartmentUpdate, DepartmentInsert")]
    public sealed class Migration7832 : TransactedMigration
    {
        private const string SqlStatement = @"
ALTER procedure [Security].[DepartmentDelete]
	@i_DepartmentID bigint,
	@ModifiedBy bigint, 
	@ModifiedOn smalldatetime
AS
SET NOCOUNT ON

Declare @lft int
SELECT	@lft=LeftBorder FROM Departments WHERE ID=@i_DepartmentID and IsDeleted=0

Update Departments Set IsDeleted=1, ModifiedBy=@ModifiedBy, ModifiedOn=@ModifiedOn WHERE ID=@i_DepartmentID 
	
UPDATE Departments SET LeftBorder = LeftBorder - 2
	WHERE LeftBorder >= @lft and IsDeleted=0

UPDATE Departments SET RightBorder = RightBorder - 2
	WHERE RightBorder >= @lft and IsDeleted=0
go


ALTER PROCEDURE [Security].[DepartmentUpdate]
	@i_DepartmentID		bigint,
	@i_Name				NVARCHAR(200),
	@i_ParentId			bigint,
	@i_IsActive			bit,
	@i_timestamp		timestamp,
	@ModifiedBy bigint, 
	@ModifiedOn smalldatetime
AS
SET NOCOUNT ON
UPDATE [Security].[Departments] Set 
	Name = @i_Name,
	ParentId = @i_ParentId,
	IsActive = @i_IsActive,
	ModifiedBy = @ModifiedBy,
	ModifiedOn = @ModifiedOn
WHERE Id = @i_DepartmentID and timestamp = @i_timestamp
go


ALTER procedure [Security].[DepartmentInsert] 
	@i_Name				NVARCHAR(200),
	@i_ParentID			bigint,
	@CreatedBy bigint,
	@CreatedOn smalldatetime
AS
SET NOCOUNT ON

IF(@i_ParentID is NULL)
BEGIN
	INSERT INTO Departments (Name, LeftBorder, RightBorder, CreatedBy, CreatedOn) 
		VALUES (@i_Name, 1, 2, @CreatedBy, @CreatedOn)
	SELECT SCOPE_IDENTITY() as NewDepartmentID   
	RETURN
END

Declare @rgt int
SELECT	@rgt=RightBorder FROM Departments WHERE ID=@i_ParentID and IsDeleted=0

IF(@rgt is null)
BEGIN
	RAISERROR(N'[InsertDepartment]. Запись не найдена. [@i_ParentID]=%d', 16,1, @i_ParentID)
	RETURN
END

UPDATE Departments SET RightBorder = RightBorder + 2
	WHERE RightBorder >= @rgt and IsDeleted=0

UPDATE Departments SET LeftBorder = LeftBorder + 2
	WHERE LeftBorder >= @rgt and IsDeleted=0
 
INSERT INTO Departments (Name, LeftBorder, RightBorder, ParentId, IsActive, CreatedBy, CreatedOn)
	   VALUES (@i_Name, @rgt, @rgt+1, @i_ParentID, 1, @CreatedBy, @CreatedOn)

SELECT SCOPE_IDENTITY() as NewDepartmentID
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(SqlStatement);
        }
    }
}
