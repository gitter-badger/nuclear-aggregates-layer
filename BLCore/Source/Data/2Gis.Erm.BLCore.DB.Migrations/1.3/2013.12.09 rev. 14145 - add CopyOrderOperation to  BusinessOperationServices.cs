using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(14145, "Добавляем операцию копирования заказа в таблицу BusinessOperationServices", "y.baranihin")]
    public class Migration14145 : TransactedMigration
    {
        private const int descriptor = 0;
        private const int CopyOrderIdentity = 15113;
        private const int ExportFlowOrdersOrderServiceCode = 5;

        private const string InsertTemplate =
            @"if not exists(select * from [Shared].[BusinessOperationServices] Where Descriptor = {0} AND Operation={1} AND Service={2})
                                                insert into [Shared].[BusinessOperationServices] ([Descriptor] ,[Operation], [Service]) values ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, CopyOrderIdentity, ExportFlowOrdersOrderServiceCode));
        }
    }
}
