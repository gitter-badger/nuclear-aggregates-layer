using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6916, "Корректируем клиентам поле CreatedOn, закосяченное из-за бага 3184")]
    public sealed class Migration6916 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
            @"
            UPDATE Billing.Clients
            SET CreatedOn = LastQualifyTime
            WHERE
            DATEDIFF (DAY, LastQualifyTime, CreatedOn) > 1
            ");
        }
    }
}