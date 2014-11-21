using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9041, "Чистим кэш проверок на рекламные материалы")]
    public sealed class Migration9041 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"DELETE FROM [Billing].[OrderValidationResults] Where OrderValidationGroupId = 2");
        }
    }
}
