using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412231754, "Создание инфраструктуры получения Id для Identity Service", "a.tukaev")]
    public class Migration201412231754 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Properties.Resources.ISC);
        }
    }
}