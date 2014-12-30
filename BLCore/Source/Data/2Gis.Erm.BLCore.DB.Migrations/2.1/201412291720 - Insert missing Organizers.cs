using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412291720, "ERM-5524:Не отображаются созданные встречи в гриде Календаря","a.pashkin")]
    public class Migration201412291720:TransactedMigration
    {
       protected override void ApplyOverride(IMigrationContext context)
       {
           context.Database.ExecuteNonQuery(Resources.InsertMissingOrganizers_201412291720);
       }
    }
}
