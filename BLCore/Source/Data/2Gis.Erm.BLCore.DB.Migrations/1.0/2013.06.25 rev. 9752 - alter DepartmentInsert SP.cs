using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9752, "Изменяем хранимки [Security].[DepartmentInsert] и [Security].[DepartmentUpdate] в связи с переходом на явное указание идентификаторов.")]
    public sealed class Migration9752 : TransactedMigration
    {
        private const string DepartmentUpdateProcedure = @"
ALTER procedure [Security].[DepartmentInsert] 
    @i_DepartmentID	    bigint,
	@i_Name				NVARCHAR(200),
	@i_ParentID			bigint,
	@CreatedBy bigint,
	@CreatedOn smalldatetime
AS
SET NOCOUNT ON

IF(@i_ParentID is NULL)
BEGIN
	INSERT INTO Departments (Id, Name, LeftBorder, RightBorder, CreatedBy, CreatedOn) 
		VALUES (@i_DepartmentID, @i_Name, 1, 2, @CreatedBy, @CreatedOn)
	SELECT @i_DepartmentID as NewDepartmentID   
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
 
INSERT INTO Departments (Id, Name, LeftBorder, RightBorder, ParentId, IsActive, CreatedBy, CreatedOn)
	   VALUES (@i_DepartmentID, @i_Name, @rgt, @rgt+1, @i_ParentID, 1, @CreatedBy, @CreatedOn)

SELECT @i_DepartmentID as NewDepartmentID
GO

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
begin tran
	if not exists(select * from [Security].[Departments] where Id = @i_DepartmentID and Timestamp = @i_timestamp)
	begin
		RAISERROR(N'[DepartmentUpdate]. Запись не найдена или была изменена в другом потоке. [@i_DepartmentID]=%d', 16, 1, @i_DepartmentID)
		RETURN
	end

	declare @left as int, @right as int, @parent as bigint
	select @left = LeftBorder, @right = RightBorder, @parent = ParentId from [Security].[Departments] where Id = @i_DepartmentID

	if @parent <> @i_ParentId
		-- перемещение подразделения и его дочерних в другую часть дерева
		begin
			-- отрезок, в котором начально находится новое родительское подразделение
			declare @newParentLeft as int, @newParentRight as int
			select @newParentLeft = LeftBorder, @newParentRight = RightBorder from [Security].[Departments] where Id = @i_ParentId

			-- размер смещения всех подразделений, которые находятся правее точки вставки изменённого
			declare @delta as int = (@right - @left) + 1

			-- сдвигаем все подразделения, которые находятся правее точки вставки, независимо от их уровня
			update [Security].[Departments]
			set LeftBorder = LeftBorder + @delta, 
				RightBorder = RightBorder + @delta
			where LeftBorder >= @newParentRight

			-- расширяем диапазон всех новых родительских, так чтобы в них поместилось перемещаемое
			update [Security].[Departments]
			set RightBorder = RightBorder + @delta
			where LeftBorder <= @newParentLeft and RightBorder >= @newParentRight

			-- изменяемое подразделение могло 'съехать'. уточняем его положение и положение его подчинённых
			select @left = LeftBorder, @right = RightBorder from [Security].[Departments] where Id = @i_DepartmentID

			-- в созданный промежуток добавляем новое подразделение
			update [Security].[Departments]
			set LeftBorder = LeftBorder - @left + @newParentRight, -- не забываем, @newParentRight - это граница нового родителя до того, как она была сдвинута вправо
				RightBorder = RightBorder - @left + @newParentRight
			where LeftBorder >= @left and RightBorder <= @right -- на забываем, на предыдущем этапе наше подразделение вместе с дочерними немного сдвинулось

			-- 'окно' на месте, откуда забрали подразделение пока остаётся неубранным, оно не мешает
		end

	-- перемещение не производится, если в нем была нужда, оно уже произведено. просто изменяем парамеры сущности
	UPDATE [Security].[Departments] Set 
		Name = @i_Name,
		ParentId = @i_ParentId,
		IsActive = @i_IsActive,
		ModifiedBy = @ModifiedBy,
		ModifiedOn = @ModifiedOn
	WHERE Id = @i_DepartmentID

commit tran
GO
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(DepartmentUpdateProcedure);
        }
    }
}
