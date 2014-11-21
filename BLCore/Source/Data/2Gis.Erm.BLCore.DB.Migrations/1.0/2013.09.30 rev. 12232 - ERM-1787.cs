using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12232, "ERM-1787")]
    public sealed class Migration12232 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            INSERT INTO Shared.BusinessOperationServices (Descriptor, Operation, Service) VALUES (653315513, 5, 11)
            ");
        }
    }
}
