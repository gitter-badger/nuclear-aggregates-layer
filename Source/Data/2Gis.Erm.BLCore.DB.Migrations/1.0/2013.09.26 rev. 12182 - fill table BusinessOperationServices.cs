using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12182, "Заполнение BusnessOperationServices")]
    public class Migration12182 : TransactedMigration
    {
        private const string InsertStatements = @"
-- Обработка операций выбора в белый список, как они были залогированы раньше (потом можно удалить)
insert into [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
values (-57837337, 18601, 4)
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
