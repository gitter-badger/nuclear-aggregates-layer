using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9345, "Заполнение BusnessOperationServices")]
    public class Migration9345 : TransactedMigration
    {
        private const string InsertStatements = @"
-- floworders.order
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (653315513, 110, 5)

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
