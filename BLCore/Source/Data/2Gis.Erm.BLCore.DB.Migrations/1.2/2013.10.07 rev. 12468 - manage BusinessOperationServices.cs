using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12468, "Управление BusnessOperationServices")]
    public class Migration12468 : TransactedMigration
    {
        private const string InsertStatements = @"
-- Отключаем обработку DeactivateIdentity (8), User (-918089089) для потока заказов ExportFlowOrdersOrder (5)
delete from Shared.BusinessOperationServices where Operation = 8 and Service = 5 and Descriptor = -918089089

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
