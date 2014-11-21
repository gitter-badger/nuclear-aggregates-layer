using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22797, "Alter BOS for Advertisement export", "y.baranikhin")]
    public class Migration22797 : TransactedMigration
    {
        public const int ResetAdvertisementElementToDraftIdentity = 31604;
        public const int TransferAdvertisementElementToReadyForValidationIdentity = 31605;

        public const long EmptyDescriptor = 0;

        public const int ExportFlowOrdersAdvMaterialService = 4;

        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Operation = {0} AND Descriptor = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Operation, Descriptor, Service)
VALUES ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate,
                                                           ResetAdvertisementElementToDraftIdentity,
                                                           EmptyDescriptor,
                                                           ExportFlowOrdersAdvMaterialService));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate,
                                                           TransferAdvertisementElementToReadyForValidationIdentity,
                                                           EmptyDescriptor,
                                                           ExportFlowOrdersAdvMaterialService));
        }
    }
}