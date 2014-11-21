using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7799, "Удаляем запрещенные комбинации символов из текста рекламных материалов")]
    public sealed class Migration7799 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string query = @"UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '\r', '') where text like '%\r%' 
UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '\n', '') where text like '%\n%'   

UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '\p', '') where text like '%\p%' 
UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '\i', '') where text like '%\i%' 

UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '/p', '') where text like '%/p' 
UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, '/i', '') where text like '%/i' 

UPDATE [Billing].[AdvertisementElements] SET Text = REPLACE(Text, char(160), ' ')  where text like '%'+char(160)+'%'
";
            context.Connection.ExecuteNonQuery(query);
        }
    }
}
