using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(21673, "Alter BOS for flowOrders export", "y.baranihin")]
    public class Migration21673 : TransactedMigration
    {
        private const int CreateOperation = 30;
        private const int UpdateOperation = 31;
        private const int DeactivateOperation = 8;
        private const int IntegrationService = 18;
        private const long DenialReasonDescriptor = 1424796032;

        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Operation = {0} AND Descriptor = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Operation, Descriptor, Service)
VALUES ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, CreateOperation, DenialReasonDescriptor, IntegrationService));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, UpdateOperation, DenialReasonDescriptor, IntegrationService));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, DeactivateOperation, DenialReasonDescriptor, IntegrationService));
        }
    }
}