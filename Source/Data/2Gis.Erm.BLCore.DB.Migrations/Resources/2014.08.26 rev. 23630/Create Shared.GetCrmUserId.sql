CREATE FUNCTION Shared.GetCrmUserId(@id bigint)
RETURNS UNIQUEIDENTIFIER
AS
BEGIN

    DECLARE @CrmId UNIQUEIDENTIFIER;

    SELECT @CrmId = [SystemUserId]
    FROM [Security].[Users]	
		LEFT OUTER JOIN	[DoubleGis_MSCRM].[dbo].[SystemUserBase] 
		ON [DomainName] LIKE N'%\' + Account COLLATE database_default
    WHERE Id = @id

	RETURN @CrmId

END
