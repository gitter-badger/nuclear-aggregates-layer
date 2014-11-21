-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Security].[CheckUserParentnessRecursion]
	@userId bigint,
	@proposedParentId bigint
AS
	DECLARE @closureCount INT;
    WITH userParentCTE(userId, userName, parentId, level)
    AS
    (
		-- Anchor member definition
        SELECT U.Id, U.Account, U.ParentId, 0 AS Level
        FROM [Security].[Users] U
        WHERE U.Id=@userId
        UNION ALL
		-- Statement that executes the CTE
        SELECT U.Id, U.Account, U.ParentId, Level+1 AS Level
        FROM [Security].[Users] U
        INNER JOIN userParentCTE ON U.ParentId = userParentCTE.userId
        WHERE level<99
    )

    SELECT @closureCount = COUNT(*)
    FROM userParentCTE cte
    WHERE cte.userId = @proposedParentId

    SELECT @closureCount


