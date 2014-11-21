using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9115, "Очистка таблиц ServiceBusExportedBusinessOperations, PerformedBusinessOperations")]
    public class Migration9115 : TransactedMigration
    {
        private const string InsertStatements = @"
delete from [Integration].[ServiceBusExportedBusinessOperations]
delete from [Shared].[PerformedBusinessOperations]
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
