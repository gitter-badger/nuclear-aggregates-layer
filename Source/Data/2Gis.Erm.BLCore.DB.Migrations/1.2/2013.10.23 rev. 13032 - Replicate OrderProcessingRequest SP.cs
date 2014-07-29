using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13032, "SP для репликации OrderProcessingRequest")]
    public class Migration13032 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources._2013_10_23_rev__13032___Replicate_OrderProcessingRequest_SP);
        }
    }
}