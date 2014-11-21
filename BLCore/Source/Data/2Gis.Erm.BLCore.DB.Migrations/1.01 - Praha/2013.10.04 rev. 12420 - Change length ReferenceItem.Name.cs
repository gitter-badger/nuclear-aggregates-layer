using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._01___Praha
{
    [Migration(12420, "Увеличение максимальной длины поля ReferenceItem.Name")]
    public sealed class Migration12420 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var referenceItemTable = context.Database.GetTable(ErmTableNames.ReferenceItems);

            if (referenceItemTable == null)
            {
                return;
            }

            var nameColumn = referenceItemTable.Columns["Name"];

            if (nameColumn == null)
            {
                return;
            }

            nameColumn.DataType = DataType.NVarCharMax;
            nameColumn.Nullable = false;
            nameColumn.Alter();
        }
    }
}
