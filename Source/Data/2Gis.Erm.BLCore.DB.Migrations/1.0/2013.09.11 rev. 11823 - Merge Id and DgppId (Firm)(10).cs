using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11823, "Объединяем колонки Id и DgppId в таблице Firms (10/10)")]
    public class Migration11823 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 30 * 60 * 1000;
            context.Output.WriteLine("Выполняю обновление идентификаторов");
            context.Output.WriteLine("0%");
            for (var i = 1; i <= 10; i++)
            {
                context.Database.ExecuteNonQuery("update TOP (1) PERCENT BusinessDirectory.Firms set Id = DgppId where DgppId is not null AND Id!=DgppId");
                context.Output.WriteLine((10 * i) + "%");
            }

            context.Output.WriteLine("Поиск необновленных записей");
            context.Database.ExecuteNonQuery("update BusinessDirectory.Firms set Id = DgppId where DgppId is not null AND Id!=DgppId");

            context.Output.WriteLine("Восстановление первоначальных настроек, удаление колонки DgppId, перестроение индексов и остальная фигня");

            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.ActivityInstances, ErmTableNames.Firms, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Advertisements, ErmTableNames.Firms, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Clients, ErmTableNames.Firms, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Deals, ErmTableNames.Firms, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmAddresses, ErmTableNames.Firms, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Orders, ErmTableNames.Firms, ForeignKeyAction.NoAction);

            var firmsTable = context.Database.GetTable(ErmTableNames.Firms);
            var dgppIdColumn = firmsTable.Columns["DgppId"];

            var uqFirmsDgppIdUniqueIndex = firmsTable.Indexes["UQ_Firms_DgppId"];
            if (uqFirmsDgppIdUniqueIndex != null)
            {
                uqFirmsDgppIdUniqueIndex.Drop();
            }

            var indexesToDrop = firmsTable.Indexes.Cast<Index>()
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
