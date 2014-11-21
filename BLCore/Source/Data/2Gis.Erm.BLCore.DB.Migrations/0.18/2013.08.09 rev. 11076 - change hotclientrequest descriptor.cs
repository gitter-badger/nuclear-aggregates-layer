using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11076, "Актуализация дескриптора HotClientRequest")]
    public sealed class Migration11076 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int oldDescriptorValue = -944632035;
            const int exportToMsCrmServiceCode = 10;
            const int newDescriptorValue = 1555675434;
            context.Connection.ExecuteNonQuery(
                string.Format(
                    @"UPDATE [Shared].[BusinessOperationServices] SET Descriptor = {2} WHERE Descriptor = {0} AND Service = {1}",
                    oldDescriptorValue,
                    exportToMsCrmServiceCode,
                    newDescriptorValue));
        }
    }
}
