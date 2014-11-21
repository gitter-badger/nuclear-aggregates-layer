-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Security].[ReplicateUser]
	@Id bigint = NULL
WITH EXECUTE AS CALLER
AS

    SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;

	SET XACT_ABORT ON;

	--Логин пользователя в системе erm, даёт нам пользователя в crm
	declare @login as nvarchar(255)
	select @login = Account from Security.Users where Security.Users.Id = @Id

	--идентификатор в crm
	declare @crmUserId as uniqueidentifier
	select @crmUserId = [SystemUserId] from DoubleGis_MSCRM.dbo.SystemUserBase where DomainName like '%\' + @login

	--получаем логин руководителя
	declare @parentLogin as nvarchar(255)
	select @parentLogin = ParentUsers.Account 
	from Security.Users inner join
		Security.Users as ParentUsers on ParentUsers.Id = Users.ParentId
	where Users.Id = @Id

	--по этому логину определяем пользователя crm
	declare @crmParentUserId as uniqueidentifier
	select @crmParentUserId = [SystemUserId] from DoubleGis_MSCRM.dbo.SystemUserBase where DomainName like '%\' + @parentLogin

	--и обновляем запись пользователя в crm
	update DoubleGis_MSCRM.dbo.SystemUserBase set ParentSystemUserId = @crmParentUserId where SystemUserId = @crmUserId

	RETURN 1;



