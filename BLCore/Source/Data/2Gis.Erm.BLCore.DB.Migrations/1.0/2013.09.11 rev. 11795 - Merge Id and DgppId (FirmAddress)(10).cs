using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11795, "Объединяем колонки Id и DgppId в таблице FirmAddresses (10/10)")]
    public class Migration11795 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 30 * 60 * 1000;
            context.Output.WriteLine("Выполняю обновление идентификаторов");
            context.Output.WriteLine("0%");
            for (var i = 1; i <= 10; i++)
            {
                context.Database.ExecuteNonQuery("update TOP (1) PERCENT BusinessDirectory.FirmAddresses set Id = DgppId where DgppId is not null AND Id!=DgppId");
                context.Output.WriteLine((10 * i) + "%");
            }

            context.Output.WriteLine("Поиск необновленных записей");
            context.Database.ExecuteNonQuery("update BusinessDirectory.FirmAddresses set Id = DgppId where DgppId is not null AND Id!=DgppId");

            context.Output.WriteLine("Восстановление первоначальных настроек, удаление колонки DgppId, перестроение индексов и остальная фигня");
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryFirmAddresses, ErmTableNames.FirmAddresses, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmAddressServices, ErmTableNames.FirmAddresses, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmContacts, ErmTableNames.FirmAddresses, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.OrderPositionAdvertisement, ErmTableNames.FirmAddresses, ForeignKeyAction.NoAction);

            var firmAddressesTable = context.Database.GetTable(ErmTableNames.FirmAddresses);
            var dgppIdColumn = firmAddressesTable.Columns["DgppId"];

            var uqFirmAddressesDgppIdUniqueIndex = firmAddressesTable.Indexes["UQ_FirmAddresses_DgppId"];
            if (uqFirmAddressesDgppIdUniqueIndex != null)
            {
                uqFirmAddressesDgppIdUniqueIndex.Drop();
            }

            var ixFirmAddressesIdDgppIdIndex = firmAddressesTable.Indexes["IX_FirmAddresses_Id_DgppId"];
            if (ixFirmAddressesIdDgppIdIndex != null)
            {
                ixFirmAddressesIdDgppIdIndex.Drop();
            }

            var indexesToDrop = firmAddressesTable.Indexes.Cast<Index>()
                                                        .Where(x => x.IndexedColumns.Contains("DgppId") && x.IndexKeyType != IndexKeyType.DriPrimaryKey)
                                                        .ToArray();

            var indexesToCreate = indexesToDrop.Select(x => EntityCopyHelper.ReplaceIndexedColumn(x, "DgppId", "Id")).ToArray();

            foreach (var index in indexesToDrop)
            {
                index.Drop();
            }

            dgppIdColumn.Drop();

            foreach (var index in indexesToCreate)
            {
                index.Create();
            }
        }
    }
}
