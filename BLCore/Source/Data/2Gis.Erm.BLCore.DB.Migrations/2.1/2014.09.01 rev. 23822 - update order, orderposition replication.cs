using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23822, "Значение скидки в позиции заказа для расширенного поиска", "a.rechkalov")]
    public class Migration23822 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.ReplicateOrder_23822);
            context.Database.ExecuteNonQuery(Resources.ReplicateOrderPosition_23822);
        }
    }
}
