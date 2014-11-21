using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11709, "Обновляем статусную модель сборки - ReleaseInfo.Status")]
    public class Migration11709 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery("UPDATE [Billing].[ReleaseInfos] SET [Status] = 3 WHERE Status = 1");
        }
    }
}