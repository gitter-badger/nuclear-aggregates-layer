using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4109, "Создание новой хранимой процедуры для репликации изменений в территориях и руководителе пользователя")]
    public sealed class Migration4109 : TransactedMigration
    {
        #region SP Text

        private const string StoredProcedureTextTemplate = @"
    SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;

	SET XACT_ABORT ON;

	--Логин пользователя в системе erm, даёт нам пользователя в crm
	declare @login as nvarchar(255)
	select @login = Account from Security.Users where Security.Users.Id = @Id

	--идентификатор в crm
	declare @crmUserId as uniqueidentifier
	select @crmUserId = [SystemUserId] from {0}.dbo.SystemUserBase where DomainName like '%\' + @login

	--сначала удалим все территории пользователя из crm
	delete from {0}.dbo.dg_systemuser_dg_territoryBase where systemuserid = @crmUserId

	--затем добавим те, которые прописаны в erm
	insert into {0}.dbo.dg_systemuser_dg_territoryBase(dg_systemuser_dg_territoryId, systemuserid, dg_territoryid) 
	select NEWID(), @crmUserId, Territories.ReplicationCode
	from Security.UserTerritories inner join
			BusinessDirectory.Territories on Territories.Id = UserTerritories.TerritoryId
	where UserTerritories.UserId = @Id

	--получаем логин руководителя
	declare @parentLogin as nvarchar(255)
	select @parentLogin = ParentUsers.Account 
	from Security.Users inner join
		Security.Users as ParentUsers on ParentUsers.Id = Users.ParentId
	where Users.Id = @Id

	--по этому логину определяем пользователя crm
	declare @crmParentUserId as uniqueidentifier
	select @crmParentUserId = [SystemUserId] from DoubleGis{0}_MSCRM.dbo.SystemUserBase where DomainName like '%\' + @parentLogin

	--и обновляем запись пользователя в crm
	update DoubleGis{0}_MSCRM.dbo.SystemUserBase set ParentSystemUserId = @crmParentUserId where SystemUserId = @crmUserId

	RETURN 1;";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var spTextBody = string.Format(StoredProcedureTextTemplate, context.CrmDatabaseName);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateUser, spTextBody);
        }
    }
}
