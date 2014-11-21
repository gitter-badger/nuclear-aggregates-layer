using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8636, "Alter на DepartmentUpdate")]
    public sealed class Migration8636 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["DepartmentUpdate", ErmSchemas.Security];
            sp.TextBody = @"SET NOCOUNT ON
begin tran
	if not exists(select * from [Security].[Departments] where Id = @i_DepartmentID and Timestamp = @i_timestamp)
	begin
		RAISERROR(N'[DepartmentUpdate]. Запись не найдена или была изменена в другом потоке. [@i_DepartmentID]=%d', 16, 1, @i_DepartmentID)
		RETURN
	end

	declare @left as int, @right as int, @parent as int
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
";

            sp.Alter();
        }
    }
}
