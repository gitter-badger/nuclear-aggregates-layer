using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23049, "Удаляем несколько записей из таблицы [Shared].[BusinessOperationServices]", "y.baranikhin")]
    public class Migration23049 : TransactedMigration
    {
        private const long ExportFlowCardExtensionsCardCommercial = 2;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var script = string.Format(
                "DELETE FROM [Shared].[BusinessOperationServices] Where Service = {0}",
                ExportFlowCardExtensionsCardCommercial);

            context.Connection.ExecuteNonQuery(script);
        }
    }
}