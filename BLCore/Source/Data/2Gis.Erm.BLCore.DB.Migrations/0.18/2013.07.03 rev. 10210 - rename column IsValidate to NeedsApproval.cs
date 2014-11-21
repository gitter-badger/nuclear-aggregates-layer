using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10210, "Меняю название столбца IsValidate to NeedsApproval")]
    public sealed class Migration10210 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.AdvertisementElementTemplates.Name, ErmTableNames.AdvertisementElementTemplates.Schema];
            var column = table.Columns["IsValidate"];
            if (column != null)
            {
                column.Rename("NeedsValidation");
            }
        }
    }
}
