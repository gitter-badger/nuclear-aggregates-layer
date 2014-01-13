using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11981, "Фикс ERM-1630")]
    public class Migration11981 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            -- replace ',' to 'x' and ';' to '|'
            UPDATE Billing.AdvertisementElementTemplates
            SET
            ImageDimensionRestriction = REPLACE(REPLACE(ImageDimensionRestriction, ',', 'x'), ';', '|')
            WHERE
            ImageDimensionRestriction IS NOT NULL

            -- trim right '|' character
            UPDATE Billing.AdvertisementElementTemplates
            SET
            ImageDimensionRestriction = LEFT(ImageDimensionRestriction, LEN(ImageDimensionRestriction) - 1)
            WHERE
            ImageDimensionRestriction IS NOT NULL
            AND
            RIGHT(ImageDimensionRestriction, 1) = '|'

            -- expand '|' to ' | '
            UPDATE Billing.AdvertisementElementTemplates
            SET
            ImageDimensionRestriction = REPLACE(ImageDimensionRestriction, '|', ' | ')
            WHERE
            ImageDimensionRestriction IS NOT NULL

            -- remove restrictions for images
            UPDATE Billing.AdvertisementElementTemplates
            SET
            FileExtensionRestriction =
            REPLACE(
            REPLACE(
            REPLACE(
            REPLACE(
            REPLACE(
            REPLACE(
            REPLACE(
            REPLACE(
            FileExtensionRestriction
            , '.', '')
            , ' ', '')
            , ',', '|')
            , 'bmp', '')
            , 'gif', 'gif')
            , 'png', 'png')
            , 'gif|png', 'png|gif')
            , 'chm', 'chm')
            WHERE
            FileExtensionRestriction IS NOT NULL

            -- trim right '|' character
            UPDATE Billing.AdvertisementElementTemplates
            SET
            FileExtensionRestriction = LEFT(FileExtensionRestriction, LEN(FileExtensionRestriction) - 1)
            WHERE
            FileExtensionRestriction IS NOT NULL
            AND
            RIGHT(FileExtensionRestriction, 1) = '|'

            -- expand '|' to ' | '
            UPDATE Billing.AdvertisementElementTemplates
            SET
            FileExtensionRestriction = REPLACE(FileExtensionRestriction, '|', ' | ')
            WHERE
            FileExtensionRestriction IS NOT NULL

            ");
        }
    }
}
