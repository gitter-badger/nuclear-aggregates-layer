using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9360, "Заполнение BusnessOperationServices")]
    public class Migration9360 : TransactedMigration
    {
        private const string InsertStatements = @"
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (653315513, 110, 9)

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
