using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13485, "Исправление типов полей в OrderProcessingRequest")]
    public sealed class Migration13485 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            // На момент написания миграции в боевой базе записей в этой таблице нет, 
            // поэтому можно спокойно менять нулябельность колонок

            SetNotNullablePropertyForColumn(table.Columns["SourceOrganizationUnitId"]);
            SetNotNullablePropertyForColumn(table.Columns["BeginDistributionDate"]);
            SetNotNullablePropertyForColumn(table.Columns["FirmId"]);
            SetNotNullablePropertyForColumn(table.Columns["LegalPersonProfileId"]);
        }

        private static void SetNotNullablePropertyForColumn(Column column)
        {
            if (column == null)
            {
                return;
            }

            column.Nullable = false;
            column.Alter();
        }
    }
}
