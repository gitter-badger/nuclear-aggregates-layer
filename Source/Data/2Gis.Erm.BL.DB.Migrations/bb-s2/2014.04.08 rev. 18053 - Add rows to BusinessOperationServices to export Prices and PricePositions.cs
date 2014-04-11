using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(18053, "Добавляем записи в таблицу [Shared].[BusinessOperationServices], чтобы поддержать экспорт прайс-листов и их позиций", "y.baranihin")]
    public sealed class Migration18053 : TransactedMigration
    {
        private const int EmptyDescriptor = 0;
        private const int ExportFlowPriceListsPriceListService = 12;
        private const int ReplacePriceOperation = 15505;
        private const int ActivateOperation = 2;
        private const int DeleteOperation = 9;
        private const int PricePositionDescriptor = -1270880171;
        private const int ExportFlowPriceListsPriceListPositionService = 13;
        private const int CopyPricePositionOperation = 15401;


        private const string InsertTemplate = @"If not exists(select * from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2})
                                                insert into Shared.BusinessOperationServices(Descriptor, Operation, Service) Values({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddRowToBusinessOperationServices(context.Connection, EmptyDescriptor, ReplacePriceOperation, ExportFlowPriceListsPriceListService);
            AddRowToBusinessOperationServices(context.Connection, PricePositionDescriptor, ActivateOperation, ExportFlowPriceListsPriceListPositionService);
            AddRowToBusinessOperationServices(context.Connection, PricePositionDescriptor, DeleteOperation, ExportFlowPriceListsPriceListPositionService);
            AddRowToBusinessOperationServices(context.Connection, EmptyDescriptor, CopyPricePositionOperation, ExportFlowPriceListsPriceListPositionService);
        }

        private void AddRowToBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, operation, service));
        }
    }
}
