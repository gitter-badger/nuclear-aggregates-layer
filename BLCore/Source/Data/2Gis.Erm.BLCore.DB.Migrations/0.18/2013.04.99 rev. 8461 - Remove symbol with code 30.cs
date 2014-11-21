using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8461, "Удаляем символ с кодом 30 из текста рекламных материалов")]
    public sealed class Migration8461 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, char(30), '')  where text like '%'+char(30)+'%'");
        }
    }
}
