using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(18055, "Удаляем записи из таблицы [Shared].[BusinessOperationServices], чтобы поддержать экспорт прайс-листов и их позиций", "y.baranihin")]
    public sealed class Migration18055 : TransactedMigration
    {
        private const int ActivateOperation = 2;
        private const int DeleteOperation = 9;
        private const int DeactivateOperation = 8;
        private const int ExportFlowPriceListsPriceListPositionService = 13;
        private const int PriceDescriptor = -965374784;


        private const string InsertTemplate = @"delete from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddRowToBusinessOperationServices(context.Connection, PriceDescriptor, ActivateOperation, ExportFlowPriceListsPriceListPositionService);
            AddRowToBusinessOperationServices(context.Connection, PriceDescriptor, DeactivateOperation, ExportFlowPriceListsPriceListPositionService);
            AddRowToBusinessOperationServices(context.Connection, PriceDescriptor, DeleteOperation, ExportFlowPriceListsPriceListPositionService);
        }

        private void AddRowToBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, operation, service));
        }
    }
}
