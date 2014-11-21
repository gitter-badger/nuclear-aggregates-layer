using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(18051, "Добавляем записи в таблицу [Shared].[BusinessOperationServices], чтобы поддержать экспорт позиций прайс-листов", "y.baranihin")]
    public sealed class Migration18051 : TransactedMigration
    {
        private const int PricePositionDescriptor = -1270880171;
        private const int ExportFlowPriceListsPriceListPositionService = 13;
        private const int CreateOperation = 30;
        private const int UpdateOperation = 31;


        private const string InsertTemplate = @"If not exists(select * from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2})
                                                insert into Shared.BusinessOperationServices(Descriptor, Operation, Service) Values({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddRowToBusinessOperationServices(context.Connection, PricePositionDescriptor, CreateOperation, ExportFlowPriceListsPriceListPositionService);
            AddRowToBusinessOperationServices(context.Connection, PricePositionDescriptor, UpdateOperation, ExportFlowPriceListsPriceListPositionService);
        }

        private void AddRowToBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, operation, service));
        }
    }
}
