using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12242, "Сбрасываем очередь репликации (ERM-1780)")]
    public class Migration12242 : TransactedMigration
    {
        private const string UpdateStatement = @"
Update [Shared].[CrmReplicationInfo] Set LastTimestamp = (Select Max(Timestamp) From BusinessDirectory.Territories) Where Entity = 'Territory'
Update [Shared].[CrmReplicationInfo] Set LastTimestamp = (Select Max(Timestamp) From BusinessDirectory.Firms) Where Entity = 'Firm'
Update [Shared].[CrmReplicationInfo] Set LastTimestamp = (Select Max(Timestamp) From BusinessDirectory.FirmAddresses) Where Entity = 'FirmAddress'";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(UpdateStatement);
        }
    }
}
