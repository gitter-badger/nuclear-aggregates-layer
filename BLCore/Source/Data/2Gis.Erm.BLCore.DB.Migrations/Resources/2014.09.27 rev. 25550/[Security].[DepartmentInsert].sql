ALTER PROCEDURE [Security].[DepartmentInsert] @i_DepartmentID BIGINT,
@i_Name NVARCHAR(200),
@i_ParentID BIGINT,
@CreatedBy BIGINT,
@CreatedOn SMALLDATETIME,

-- ignored, but needed parameters
@LeftBorder INT,
@RightBorder INT,
@IsActive BIT,
@IsDeleted BIT,
@ModifiedBy BIGINT,
@ModifiedOn SMALLDATETIME

AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Result TABLE (
		Timestamp BINARY(8)
	);
	IF (@i_ParentID IS NULL)
	BEGIN
		INSERT INTO Departments (id, Name, LeftBorder, RightBorder, CreatedBy, CreatedOn)
		OUTPUT INSERTED.Timestamp INTO @Result
			VALUES (@i_DepartmentID, @i_Name, 1, 2, @CreatedBy, @CreatedOn)

		SELECT Timestamp
		FROM @Result
		RETURN
	END

	DECLARE @rgt INT
	SELECT @rgt = RightBorder
	FROM Departments
	WHERE id = @i_ParentID
	AND IsDeleted = 0

	IF (@rgt IS NULL)
	BEGIN
		RAISERROR (N'[InsertDepartment]. Запись не найдена. [@i_ParentID]=%d', 16, 1, @i_ParentID)
		SELECT NULL AS Timestamp
		RETURN
	END

	UPDATE Departments
	SET RightBorder = RightBorder + 2
	WHERE RightBorder >= @rgt
	AND IsDeleted = 0

	UPDATE Departments
	SET LeftBorder = LeftBorder + 2
	WHERE LeftBorder >= @rgt
	AND IsDeleted = 0

	INSERT INTO Departments (id, Name, LeftBorder, RightBorder, ParentId, IsActive, CreatedBy, CreatedOn)
	OUTPUT INSERTED.Timestamp INTO @Result
		VALUES (@i_DepartmentID, @i_Name, @rgt, @rgt + 1, @i_ParentID, 1, @CreatedBy, @CreatedOn)

	SELECT Timestamp
	FROM @Result
END