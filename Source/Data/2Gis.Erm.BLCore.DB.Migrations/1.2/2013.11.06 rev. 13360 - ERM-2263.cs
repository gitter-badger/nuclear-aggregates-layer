using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    // Миграция пустая т.к. она использовала админску функцию, на которую мы не можем положиться. 
    // Миграция 13432 отменит действия данной миграции там, где она все-таки выполнилась. 
    // Миграция 13432 также выполнит то, что должна была сделать данная миграция 
    [Migration(13360, "Настройка прав для пользователя ЛК")]
    public sealed class Migration13360 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {

        }
    }
}
