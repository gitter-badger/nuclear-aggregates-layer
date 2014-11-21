using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10437, "Обновляем ModifiedOn у некоторых фирм в CRM")]
    public sealed class Migration10437 : TransactedMigration, INonDefaultDatabaseMigration
    {
        private const string UpdateModifiedOn =
            @"UPDATE dbo.Dg_firmBase SET ModifiedOn = DATEADD(Second,-1,ModifiedOn) WHERE Dg_firmId in 
(SELECT fb.Dg_firmId FROM {0}.BusinessDirectory.Firms f 
inner join {0}.BusinessDirectory.Territories t ON f.TerritoryId = t.Id
inner join dbo.Dg_firmBase fb ON f.ReplicationCode = fb.Dg_firmId AND f.ModifiedOn = fb.ModifiedOn
inner join dbo.Dg_firmExtensionBase feb ON feb.Dg_firmId = fb.Dg_firmId and feb.dg_territory != t.ReplicationCode)";

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var query = string.Format(UpdateModifiedOn, context.ErmDatabaseName);
            context.Connection.ExecuteNonQuery(query);
        }
    }
}
