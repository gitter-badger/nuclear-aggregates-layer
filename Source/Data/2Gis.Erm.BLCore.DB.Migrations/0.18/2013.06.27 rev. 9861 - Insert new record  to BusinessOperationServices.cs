using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9861, "Добавляем запись в BusnessOperationServices")]
    public class Migration9861 : TransactedMigration
    {
        private const string InsertStatements = @"
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-944632035, 1, 10)";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
