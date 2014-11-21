using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10723, "Добавляем обработку операций списания и отмены списания сервисом выгрузки заказов")]
    public sealed class Migration10723 : TransactedMigration
    {
        public const int WithdrawalInfoDescriptor = -900885408;
        public const int ExportFlowOrdersOrderService = 5;
        public const int WithdrawalOperation = 14;
        public const int RevertWithdrawalOperation = 15;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                string.Format(
                    "IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})",
                    WithdrawalInfoDescriptor,
                    WithdrawalOperation,
                    ExportFlowOrdersOrderService));
            context.Connection.ExecuteNonQuery(
                string.Format(
                    "IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})",
                    WithdrawalInfoDescriptor,
                    RevertWithdrawalOperation,
                    ExportFlowOrdersOrderService));
        }
    }
}
