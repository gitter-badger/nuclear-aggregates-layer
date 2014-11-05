using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(24906, "Обновление ReplicateDeal с учётом изменений в 24271 и 23100", "a.rechkalov")]
    public class Migration24906 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Properties.Resources.BusinessDirectory_ReplicateDeal_24906);
        }
    }
}