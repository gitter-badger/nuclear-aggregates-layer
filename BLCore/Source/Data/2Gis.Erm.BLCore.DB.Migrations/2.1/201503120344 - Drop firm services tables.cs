using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201503120344, "удаляем таблицы, связанные с дополнительными услагами по фирме", "y.baranihin")]
    public class Migration201503120344 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.GetTable(ErmTableNames.ExportFlowCardExtensionsCardCommercial).Drop();
            context.Database.GetTable(ErmTableNames.FirmAddressServices).Drop();
            context.Database.GetTable(ErmTableNames.AdditionalFirmServices).Drop();
        }
    }
}