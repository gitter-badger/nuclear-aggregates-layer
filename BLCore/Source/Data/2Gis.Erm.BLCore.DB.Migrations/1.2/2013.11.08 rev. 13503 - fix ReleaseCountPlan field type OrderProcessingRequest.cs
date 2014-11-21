using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13503, "Исправление типа ReleaseCountPlan поля в OrderProcessingRequest")]
    public sealed class Migration13503 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            var column = table.Columns["ReleaseCountPlan"]; 
            
            if (column == null)
            {
                return;
            }

            column.DataType = new DataType(SqlDataType.SmallInt);
            column.Alter();
        }
    }
}
