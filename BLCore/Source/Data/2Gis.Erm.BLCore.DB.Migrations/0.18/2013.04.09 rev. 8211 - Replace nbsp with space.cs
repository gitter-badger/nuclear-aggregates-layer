using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8211, "Меняем неразрывные пробелы на обычные")]
    public sealed class Migration8211 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '&nbsp;', ' ') where text like '%&nbsp;%'
GO

UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, char(160), ' ')  where text like '%'+char(160)+'%'
GO");
        }
    }
}
