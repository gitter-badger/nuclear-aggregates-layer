using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8991, "Удаляем дубли ЭРМ")]
    public sealed class Migration8991 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"UPDATE [Billing].[AdvertisementElements] set IsDeleted = 1, ModifiedBy = 1, ModifiedOn = GETUTCDATE() where id in (
 11792
,12687
,85044
,86175
,86471
,87835
,572347
,572833
,12421
,12423
,572339
,572830)");
        }
    }
}
