using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12814, "Удаляем OwnerCode в таблице Shared.LocalMessages")]
    public class Migration12814 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var localMessagesTable = context.Database.GetTable(ErmTableNames.LocalMessages);
            if (localMessagesTable != null)
            {
                var ownerCode = localMessagesTable.Columns["OwnerCode"];
                if (ownerCode != null)
                {
                    ownerCode.Drop();
                }
            }
        }
    }
}