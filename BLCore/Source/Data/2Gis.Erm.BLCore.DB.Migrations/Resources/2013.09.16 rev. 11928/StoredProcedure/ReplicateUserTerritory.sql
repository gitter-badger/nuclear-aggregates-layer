-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Security].[ReplicateUserTerritory]
	@Id bigint = NULL
AS

SET NOCOUNT ON;
	
IF @Id IS NULL
	RETURN 0;

SET XACT_ABORT ON;

--Логин пользователя в системе erm, даёт нам пользователя в crm
declare @login as nvarchar(255)
select @login = Account from Security.Users inner join Security.UserTerritories on Users.Id = UserTerritories.UserId where Security.UserTerritories.Id = @Id

--идентификатор в crm
declare @crmUserId as uniqueidentifier
select @crmUserId = [SystemUserId] from DoubleGis_MSCRM.dbo.SystemUserBase where DomainName like N'%\' + @login

--выгребаем произошедшие изменения
declare @isDeleted as bit
declare @replicationCode as uniqueidentifier
select @replicationCode = Territories.ReplicationCode, @isDeleted = UserTerritories.IsDeleted
from Security.UserTerritories inner join
		BusinessDirectory.Territories on Territories.Id = UserTerritories.TerritoryId
where UserTerritories.Id = @Id

--и накатываем на базу crm
if @isDeleted = 1
	delete from DoubleGis_MSCRM.dbo.dg_systemuser_dg_territoryBase 
	where systemuserid = @crmUserId and dg_territoryid = @replicationCode
else if NOT EXISTS (select 1 from DoubleGis_MSCRM.dbo.dg_systemuser_dg_territoryBase WHERE systemuserid = @crmUserId and dg_territoryid = @replicationCode)
	insert into DoubleGis_MSCRM.dbo.dg_systemuser_dg_territoryBase(dg_systemuser_dg_territoryId, systemuserid, dg_territoryid)
	values (NEWID(), @crmUserId, @replicationCode)

RETURN 1;
