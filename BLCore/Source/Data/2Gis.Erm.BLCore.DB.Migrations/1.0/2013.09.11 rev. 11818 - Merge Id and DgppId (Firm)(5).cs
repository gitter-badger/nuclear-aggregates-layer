﻿using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11818, "Объединяем колонки Id и DgppId в таблице Firms (5/10)")]
    public class Migration11818 : TransactedMigration
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
        }
    }
}
