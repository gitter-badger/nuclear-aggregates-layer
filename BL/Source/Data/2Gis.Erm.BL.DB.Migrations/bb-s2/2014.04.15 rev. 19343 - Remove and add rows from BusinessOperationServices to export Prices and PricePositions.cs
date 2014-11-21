using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(19343, "Удаляем и добавляем записи из таблицы [Shared].[BusinessOperationServices], чтобы поддержать экспорт прайс-листов и их позиций", "y.baranihin")]
    public sealed class Migration19343 : TransactedMigration
    {
        private const int OperationToDelete = 15502;
        private const int OperationToAdd = 15504;
        private const int ExportFlowPriceListsPriceListService = 12;
        private const int EmptyDescriptor = 0;


        private const string DeleteTemplate = @"delete from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2}";

        private const string InsertTemplate = @"If not exists(select * from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2})
                                                insert into Shared.BusinessOperationServices(Descriptor, Operation, Service) Values({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            DeleteRowFromBusinessOperationServices(context.Connection, EmptyDescriptor, OperationToDelete, ExportFlowPriceListsPriceListService);
            AddRowToBusinessOperationServices(context.Connection, EmptyDescriptor, OperationToAdd, ExportFlowPriceListsPriceListService);
        }

        private void DeleteRowFromBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(DeleteTemplate, descriptor, operation, service));
        }

        private void AddRowToBusinessOperationServices(ServerConnection connection, int descriptor, int operation, int service)
        {
            connection.ExecuteNonQuery(string.Format(InsertTemplate, descriptor, operation, service));
        }
    }
}
