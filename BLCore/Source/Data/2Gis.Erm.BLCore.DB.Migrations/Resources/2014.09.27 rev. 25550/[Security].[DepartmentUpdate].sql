ALTER PROCEDURE [Security].[DepartmentUpdate] @i_DepartmentID BIGINT,
@i_Name NVARCHAR(200),
@i_ParentId BIGINT,
@i_IsActive BIT,
@i_timestamp TIMESTAMP,
@ModifiedBy BIGINT,
@ModifiedOn SMALLDATETIME,

-- ignored, but needed parameters
@LeftBorder INT,
@RightBorder INT,
@IsDeleted BIT,
@CreatedBy BIGINT,
@CreatedOn SMALLDATETIME,

-- used by EF
@RowsAffected INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRAN
		IF NOT EXISTS (SELECT
				*
			FROM [Security].[Departments]
			WHERE id = @i_DepartmentID
			AND TIMESTAMP = @i_timestamp)
		BEGIN
			RAISERROR (N'[DepartmentUpdate]. Запись не найдена или была изменена в другом потоке. [@i_DepartmentID]=%d', 16, 1, @i_DepartmentID)
			RETURN
		END

		DECLARE	@left AS INT,
				@right AS INT,
				@parent AS BIGINT
		SELECT	@left = LeftBorder,
				@right = RightBorder,
				@parent = ParentId
		FROM [Security].[Departments]
		WHERE id = @i_DepartmentID

		IF @parent <> @i_ParentId
		-- перемещение подразделения и его дочерних в другую часть дерева
		BEGIN
			-- отрезок, в котором начально находится новое родительское подразделение
			DECLARE	@newParentLeft AS INT,
					@newParentRight AS INT
			SELECT	@newParentLeft = LeftBorder,
					@newParentRight = RightBorder
			FROM [Security].[Departments]
			WHERE id = @i_ParentId

			-- размер смещения всех подразделений, которые находятся правее точки вставки изменённого
			DECLARE @delta AS INT = (@right - @left) + 1

			-- сдвигаем все подразделения, которые находятся правее точки вставки, независимо от их уровня
			UPDATE [Security].[Departments]
			SET	LeftBorder = LeftBorder + @delta,
				RightBorder = RightBorder + @delta
			WHERE LeftBorder >= @newParentRight

			-- расширяем диапазон всех новых родительских, так чтобы в них поместилось перемещаемое
			UPDATE [Security].[Departments]
			SET RightBorder = RightBorder + @delta
			WHERE LeftBorder <= @newParentLeft
			AND RightBorder >= @newParentRight

			-- изменяемое подразделение могло 'съехать'. уточняем его положение и положение его подчинённых
			SELECT	@left = LeftBorder,
					@right = RightBorder
			FROM [Security].[Departments]
			WHERE id = @i_DepartmentID

			-- в созданный промежуток добавляем новое подразделение
			UPDATE [Security].[Departments]
			SET	LeftBorder = LeftBorder - @left + @newParentRight, -- не забываем, @newParentRight - это граница нового родителя до того, как она была сдвинута вправо
				RightBorder = RightBorder - @left + @newParentRight
			WHERE LeftBorder >= @left
			AND RightBorder <= @right -- на забываем, на предыдущем этапе наше подразделение вместе с дочерними немного сдвинулось

		-- 'окно' на месте, откуда забрали подразделение пока остаётся неубранным, оно не мешает
		END

		-- перемещение не производится, если в нем была нужда, оно уже произведено. просто изменяем парамеры сущности
		UPDATE [Security].[Departments]
		SET	Name = @i_Name,
			ParentId = @i_ParentId,
			IsActive = @i_IsActive,
			ModifiedBy = @ModifiedBy,
			ModifiedOn = @ModifiedOn
		WHERE id = @i_DepartmentID


		SET @RowsAffected = @@rowcount
	COMMIT TRAN
END