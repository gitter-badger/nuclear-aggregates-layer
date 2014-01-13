using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5225, "Избавляемся от нулевых значений в колонке HasDocumentsDebt в заказах и договорах в CRM.")]
    public sealed class Migration5225 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey {
            get { return ErmConnectionStringKey.CrmDatabase;}
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(String.Format("UPDATE {0} SET Dg_documentsdebt = 2 WHERE Dg_documentsdebt = 0", CrmTableNames.Dg_orderExtensionBase));

            context.Connection.ExecuteNonQuery(String.Format("UPDATE {0} SET Dg_documentsdebt = 2 WHERE Dg_documentsdebt = 0 OR Dg_documentsdebt IS NULL", CrmTableNames.Dg_bargainExtensionBase));
        }
    }
}
