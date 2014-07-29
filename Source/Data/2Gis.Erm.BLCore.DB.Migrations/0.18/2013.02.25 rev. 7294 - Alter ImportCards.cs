using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7294, "Осмысленное сообщение при незнакомом коде ДГПП для подразделения")]
    public sealed class Migration7294 : TransactedMigration
    {
        // Да, тут действительно пусто. То есть совсем пусто. Подробнее в миграции №7395.
        protected override void ApplyOverride(IMigrationContext context)
        {
        }
    }
}
