using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4587, "fix ReplicateUserTerritory stored procedure")]
    public sealed class Migration4587 : TransactedMigration
    {

        #region SP Text
        private const string StoredProcedureTemplate = @"
    SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;

	SET XACT_ABORT ON;

	--Логин пользователя в системе erm, даёт нам пользователя в crm
	declare @login as nvarchar(255)
	select @login = Account from Security.Users inner join Security.UserTerritories on Users.Id = UserTerritories.UserId where Security.UserTerritories.Id = @Id

	--идентификатор в crm
	declare @crmUserId as uniqueidentifier
	select @crmUserId = [SystemUserId] from {0}.dbo.SystemUserBase where DomainName like '%\' + @login

	--выгребаем произошедшие изменения
	declare @isDeleted as bit
	declare @replicationCode as uniqueidentifier
	select @replicationCode = Territories.ReplicationCode, @isDeleted = UserTerritories.IsDeleted
	from Security.UserTerritories inner join
			BusinessDirectory.Territories on Territories.Id = UserTerritories.TerritoryId
	where UserTerritories.Id = @Id

	--и накатываем на базу crm
	if @isDeleted = 1
		delete from {0}.dbo.dg_systemuser_dg_territoryBase 
		where systemuserid = @crmUserId and dg_territoryid = @replicationCode
	else if NOT EXISTS (select 1 from {0}.dbo.dg_systemuser_dg_territoryBase WHERE systemuserid = @crmUserId and dg_territoryid = @replicationCode)
		insert into {0}.dbo.dg_systemuser_dg_territoryBase(dg_systemuser_dg_territoryId, systemuserid, dg_territoryid)
		values (NEWID(), @crmUserId, @replicationCode)

	RETURN 1;
";
        
        #endregion
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var spTextBody = string.Format(StoredProcedureTemplate, context.CrmDatabaseName);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateUserTerritory, spTextBody);
        }
    }
}
