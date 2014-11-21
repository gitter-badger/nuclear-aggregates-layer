using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11788, "Объединяем колонки Id и DgppId в таблице FirmAddresses (3/10)")]
    public class Migration11788 : TransactedMigration
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
        }
    }
}
