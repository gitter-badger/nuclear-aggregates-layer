using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(18050, "Добавляем записи в таблицу [Shared].[BusinessOperationServices], чтобы поддержать экспорт прайс-листов", "y.baranihin")]
    public sealed class Migration18050 : TransactedMigration
    {
        private const int PriceDescriptor = -965374784;
        private const int EmptyDescriptor = 0;
        private const int ExportFlowPriceListsPriceListService = 12;
        private const int CreateOperation = 30;
        private const int UpdateOperation = 31;
        private const int PublishPriceOperation = 15502;


        private const string InsertTemplate = @"If not exists(select * from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2})
                                                insert into Shared.BusinessOperationServices(Descriptor, Operation, Service) Values({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddRowToBusinessOperationServices(context.Connection, PriceDescriptor, CreateOperation, ExportFlowPriceListsPriceListService);
            AddRowToBusinessOperationServices(context.Connection, PriceDescriptor, UpdateOperation, ExportFlowPriceListsPriceListService);
            AddRowToBusinessOperationServices(context.Connection, EmptyDescriptor, PublishPriceOperation, ExportFlowPriceListsPriceListService);
        }

        private void AddRowToBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, operation, service));
        }
    }
}
