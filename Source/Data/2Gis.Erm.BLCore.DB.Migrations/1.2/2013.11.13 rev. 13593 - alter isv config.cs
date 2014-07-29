using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13593, "Исправляем отмену действий в Dynamics CRM (удалено)")]
    public class Migration13593 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
        }
    }
}