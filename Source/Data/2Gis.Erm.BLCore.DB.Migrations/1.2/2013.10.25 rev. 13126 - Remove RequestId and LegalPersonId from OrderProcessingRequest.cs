using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13126, "Удаляем поля RequestId и LegalPersonId из OrderProcessingRequests")]
    public class Migration13126 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            SetNullablePropertyForColumn(table.Columns["RequestId"]);
            SetNullablePropertyForColumn(table.Columns["LegalPersonId"]);
        }

        private static void SetNullablePropertyForColumn(Column column)
        {
            if (column == null)
            {
                return;
            }

            column.Drop();
        }
    }
}
