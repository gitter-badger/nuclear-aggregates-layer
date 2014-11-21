using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9743, "Добавление индекса на ReleasesWithdrawalsPositions")]
    public sealed class Migration9743 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ReleasesWithdrawalsPositions);

            const string indexName = @"IX_ReleasesWithdrawalsPositions_ReleasesWithdrawalId";

            var index = table.Indexes[indexName];
            if (index != null)
            {
                return;
            }

            context.Connection.ExecuteNonQuery(
                @"CREATE NONCLUSTERED INDEX [IX_ReleasesWithdrawalsPositions_ReleasesWithdrawalId] ON [Billing].[ReleasesWithdrawalsPositions]
(
       [ReleasesWithdrawalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");


        }
    }
}
