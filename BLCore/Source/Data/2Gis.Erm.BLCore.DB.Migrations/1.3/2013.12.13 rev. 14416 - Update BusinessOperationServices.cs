using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(14416, "ImportFlowCardsForErm -> ExportFlowOrdersOrder #2", "a.tukaev")]
    public class Migration14416 : TransactedMigration
    {
        private const string DeleteStatementTemplate = @"DELETE from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate, -1198296840, 9, 5));
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate, -1198296840, 30, 5));
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate, -1198296840, 31, 5));
        }
    }
}