using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8119, "Repeat theme export")]
    public sealed class Migration8119 : TransactedMigration
    {
        private const string UpdateThemeTimestamp = @"update Billing.Themes set IsDeleted = 0 where IsDeleted = 0";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(UpdateThemeTimestamp);
        }
    }
}