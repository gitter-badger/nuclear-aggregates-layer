using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5055, "Восстанавливаем репликацию UserTerritories")]
    public sealed class Migration5055 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddIsDeletedColumn(context);
            AddReplicateUserTerritory(context);
            AlterUserTerritoriesOrganizationUnitsView(context);
        }

        private static void AlterUserTerritoriesOrganizationUnitsView(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserTerritoriesOrganizationUnits", ErmSchemas.Security];
            view.TextBody = @"SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY U.id) AS Id, U.Id AS UserId, OU.Id AS OrganizationUnitId, T.Id AS TerritoryId
                         FROM Security.Users U
                         INNER JOIN Security.UserOrganizationUnits UOU ON UOU.UserId = U.Id
                         INNER JOIN Billing.OrganizationUnits OU ON OU.Id = UOU.OrganizationUnitId
                         LEFT JOIN Security.UserTerritories UT ON UT.UserId = U.Id
                         LEFT JOIN BusinessDirectory.Territories T ON T.Id = UT.TerritoryId
                         WHERE
                         (U.IsDeleted = 0 AND U.IsActive = 1)
                         AND
                         (T.IsDeleted = 0 AND T.IsActive = 1)
                         AND
                         (UT.IsDeleted = 0)
                         AND
                         (OU.IsDeleted = 0 AND OU.IsActive = 1)";
            view.Alter();
        }

        private static void AddReplicateUserTerritory(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["ReplicateUserTerritory", ErmSchemas.Security];
            if (sp != null)
                return;

            const string spTextBody = @"
SET NOCOUNT ON;
	
IF @Id IS NULL
	RETURN 0;

SET XACT_ABORT ON;

--Логин пользователя в системе erm, даёт нам пользователя в crm
declare @login as nvarchar(255)
select @login = Account from Security.Users inner join Security.UserTerritories on Users.Id = UserTerritories.UserId where Security.UserTerritories.Id = @Id

--идентификатор в crm
declare @crmUserId as uniqueidentifier
select @crmUserId = [SystemUserId] from DoubleGis_MSCRM.dbo.SystemUserBase where DomainName like '%\' + @login

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

RETURN 1;";

            ReplicationHelper.UpdateOrCreateReplicationSP(context, new SchemaQualifiedObjectName(ErmSchemas.Security, "ReplicateUserTerritory"), spTextBody);
        }

        private static void AddIsDeletedColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["UserTerritories", ErmSchemas.Security];
            var column = table.Columns["IsDeleted"];
            if (column != null)
                return;

            column = new Column(table, "IsDeleted", DataType.Bit) {Nullable = false};
            column.AddDefaultConstraint("DF_UserTerritories_IsDeleted").Text = "0";

            column.Create();
        }


    }
}