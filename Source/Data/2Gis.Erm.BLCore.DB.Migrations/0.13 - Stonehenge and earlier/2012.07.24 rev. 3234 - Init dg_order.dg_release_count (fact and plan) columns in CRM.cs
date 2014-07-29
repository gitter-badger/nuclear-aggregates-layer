using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3234, "Обновление колонок dg_release_count_plan/fact в CRM.")]
    public class Migration3234 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            String query =
@"UPDATE oeb SET 
	Dg_release_count_plan=Orders.ReleaseCountPlan
	,Dg_release_count_fact = Orders.ReleaseCountFact
	
FROM {1} AS oeb
INNER JOIN [{0}].[Billing].[Orders] ON oeb.[Dg_orderId] = [Orders].[ReplicationCode]";

            query = String.Format(query, context.ErmDatabaseName, CrmTableNames.Dg_orderExtensionBase);

            context.Connection.ExecuteNonQuery(query);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }
    }
}
