using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2460, "Проставление значений поля dg_order.dg_documentsdebt в CRM.")]
    public class Migration2460 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 180;
            String script = String.Format("UPDATE {0} SET Dg_documentsdebt = 2 WHERE Dg_hasdebt = 0",
                                          CrmTableNames.Dg_orderExtensionBase);
            context.Connection.ExecuteNonQuery(script);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }
    }
}
