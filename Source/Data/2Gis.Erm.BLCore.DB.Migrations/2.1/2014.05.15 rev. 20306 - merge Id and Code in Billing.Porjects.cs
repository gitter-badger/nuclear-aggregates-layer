using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20306, "Projects: Id and Code merge", "a.tuakev")]
    public class Migration20306 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery("update billing.projects set id = code");

            var projects = context.Database.GetTable(ErmTableNames.Projects);
            var codeColumn = projects.Columns["Code"];

            codeColumn.Drop();
        }
    }
}