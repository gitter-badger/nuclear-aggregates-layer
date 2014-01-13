using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12283, "Заполнение BusnessOperationServices")]
    public class Migration12283 : TransactedMigration
    {
        private const string InsertStatements = @"
-- ImportFirms (бывшее ImportFirmsFromDgpp)
insert into [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
values (0, 14602, 11)

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
