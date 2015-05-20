using DoubleGis.Erm.BL.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201504011053, "Изменение стабильных идентификаторов ценовых категорий", "a.rechkalov")]
    public class Migration201504011053 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.script_201504011053);
        }
    }
}