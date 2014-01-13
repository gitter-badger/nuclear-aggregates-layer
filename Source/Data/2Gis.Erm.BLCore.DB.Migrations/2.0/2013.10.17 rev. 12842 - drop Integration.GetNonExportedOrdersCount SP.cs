using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._2._0
{
    [Migration(12842, "Удаляем SP Integration.GetNonExportedOrdersCount, ставшую ненужной, после внедрения гарантированного экспорта")]
    public sealed class Migration12842: TransactedMigration
    {
        private const string SqlStatement = @"
            IF OBJECT_ID('{0}') IS NOT NULL
            DROP PROC {0}	
            GO";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(SqlStatement, "[Integration].[GetNonExportedOrdersCount]"));
        }
    }
}
