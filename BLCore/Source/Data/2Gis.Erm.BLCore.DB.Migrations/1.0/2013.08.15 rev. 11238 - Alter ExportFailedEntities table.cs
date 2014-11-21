using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11238, "ExportFailedEntities проверяем что колонка EntityId имеет тип bigint")]
    public class Migration11238 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["ExportFailedEntities", ErmSchemas.Integration];
            var column = table.Columns["EntityId"];

            if (column.DataType.SqlDataType == SqlDataType.BigInt)
            {
                return;
            }

            column.DataType = new DataType(SqlDataType.BigInt);
            column.Alter();
        }
    }
}
