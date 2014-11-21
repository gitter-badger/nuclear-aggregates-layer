using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11786, "Объединяем колонки Id и DgppId в таблице FirmAddresses (1/10)")]
    public class Migration11786 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Output.WriteLine("Выполняю предварительные настройки");
            context.Connection.StatementTimeout = 30 * 60 * 1000;

            // Миграция работает по аналогии с миграцией 10749
            // Основная идея - обновить столбец Id, включив каскадное обновление внешних ключей, т.о. все они будут иметь корректное значение
            // Далее, перебросить на Id индексы с DgppId и удалить последний
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryFirmAddresses, ErmTableNames.FirmAddresses, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmAddressServices, ErmTableNames.FirmAddresses, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.FirmContacts, ErmTableNames.FirmAddresses, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.OrderPositionAdvertisement, ErmTableNames.FirmAddresses, ForeignKeyAction.Cascade);

            context.Output.WriteLine("Выполняю обновление идентификаторов");
            context.Output.WriteLine("0%");
            for (int i = 1; i <= 10; i++)
            {
                context.Database.ExecuteNonQuery("update TOP (1) PERCENT BusinessDirectory.FirmAddresses set Id = DgppId where DgppId is not null AND Id!=DgppId");
                context.Output.WriteLine((10 * i) + "%");
            }
        }
    }
}
