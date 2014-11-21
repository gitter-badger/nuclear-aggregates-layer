using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12121, "Заполнение BusnessOperationServices")]
    public class Migration12121 : TransactedMigration
    {
        private const string InsertStatements = @"
DELETE FROM [Shared].[BusinessOperationServices] WHERE [Operation] = 18601 and [Service] = 4

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
