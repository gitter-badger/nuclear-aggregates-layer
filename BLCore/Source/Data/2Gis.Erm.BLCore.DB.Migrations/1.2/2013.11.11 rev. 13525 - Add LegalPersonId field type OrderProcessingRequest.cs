using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13525, "Добавляем в таблицу OrderProcessingRequests поле LegalPersonId")]
    public class Migration13525 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string LegalPersonIdColumnName = "LegalPersonId";

            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            if (table.Columns.Contains(LegalPersonIdColumnName))
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(11, x => new Column(x, LegalPersonIdColumnName, DataType.BigInt) { Nullable = false })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}
