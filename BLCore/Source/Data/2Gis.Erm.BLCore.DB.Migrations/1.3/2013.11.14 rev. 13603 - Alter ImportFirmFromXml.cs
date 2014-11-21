using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13603, "Актуализация ImportFirmFromXml для корректной работы с FirmAddresses из потока flowCardsForERM", "a.tukaev")]
    public class Migration13603 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.ImportFirmFromXml_13603);
        }
    }
}