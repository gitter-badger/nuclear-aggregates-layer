using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11295, "Увеличиваем длину поля ReferencePoint")]
    public sealed class Migration11295 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"ALTER TABLE [BusinessDirectory].[FirmAddresses] ALTER COLUMN ReferencePoint NVARCHAR(MAX) NULL");
        }
    }
}