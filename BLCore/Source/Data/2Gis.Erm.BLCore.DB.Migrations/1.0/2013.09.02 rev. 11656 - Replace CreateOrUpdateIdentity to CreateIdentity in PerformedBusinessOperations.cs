using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11656, "Обновление записей о выполненных бизнес-операциях ([Shared].[PerformedBusinessOperations]). Замена CreateOrUpdate на Create")]
    public class Migration11656 : TransactedMigration
    {
        private const string UpdateStatement = @"
UPDATE [Shared].[PerformedBusinessOperations]
SET [Operation] = 30 --CreateIdentity
WHERE [Operation] = 23 --CreateOrUpdateIdentity
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatement);
        }
    }
}
