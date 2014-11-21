using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13592, "Обновляем статусную модель списания - WithdrawalInfo.Status")]
    public class Migration13592 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery("UPDATE [Billing].[WithdrawalInfos] SET [Status] = 3 WHERE Status = 1");
        }
    }
}