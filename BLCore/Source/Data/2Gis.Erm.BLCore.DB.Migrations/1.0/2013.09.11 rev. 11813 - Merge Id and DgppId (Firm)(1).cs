using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11813, "Объединяем колонки Id и DgppId в таблице Firms (1/10)")]
    public class Migration11813 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Output.WriteLine("Выполняю предварительные настройки");
            context.Connection.StatementTimeout = 30 * 60 * 1000;

            // Миграция работает по аналогии с миграцией 10749
            // Основная идея - обновить столбец Id, включив каскадное обновление внешних ключей, т.о. все они будут иметь корректное значение
            // Далее, перебросить на Id индексы с DgppId и удалить последний
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.ActivityInstances, ErmTableNames.Firms, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Advertisements, ErmTableNames.Firms, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Clients, ErmTableNames.Firms, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Deals, ErmTableNames.Firms, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmAddresses, ErmTableNames.Firms, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Orders, ErmTableNames.Firms, ForeignKeyAction.Cascade);

            context.Output.WriteLine("Выполняю обновление идентификаторов");
            context.Output.WriteLine("0%");
            for (int i = 1; i <= 10; i++)
            {
                context.Database.ExecuteNonQuery("update TOP (1) PERCENT BusinessDirectory.Firms set Id = DgppId where DgppId is not null AND Id!=DgppId");
                context.Output.WriteLine((10 * i) + "%");
            }
        }
    }
}
