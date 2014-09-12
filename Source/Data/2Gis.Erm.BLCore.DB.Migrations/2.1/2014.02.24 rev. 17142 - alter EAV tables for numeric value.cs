using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(17142, "Увеличение длины полей NumericValue в DictionaryEntityPropertyInstances и BusinessEntityPropertyInstances", "d.ivanov")]
    public class Migration17142 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterPropertiesTables(context.Database.GetTable(ErmTableNames.DictionaryEntityPropertyInstances));
            AlterPropertiesTables(context.Database.GetTable(ErmTableNames.BusinessEntityPropertyInstances));
        }

        private void AlterPropertiesTables(Table propertyInstancesTable)
        {
            var column = propertyInstancesTable.Columns["NumericValue"];
            if (column == null)
            {
                return;
            }

            column.DataType = DataType.Decimal(4, 23);
            column.Alter();
        }
    }
}