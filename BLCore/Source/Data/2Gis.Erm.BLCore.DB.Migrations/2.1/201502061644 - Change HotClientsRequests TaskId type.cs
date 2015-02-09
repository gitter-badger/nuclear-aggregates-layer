using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20150206, "ERM-5696. Мы должны перевести связь между HotClientRequest и Task сущностями на идентификаторы ERM вместо CRM'ских guid'ов", "a.pashkin")]
    public class Migration201502061644 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.Change_HotClientRequest_TaskId_type);
        }
    }
}
