using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6718, "Меняем sitemap.xml (Заменяем относительные url на абсолютные https://web-app.erm.2gis.local)")]
    public class Migration6718 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration6718));
            return element;
        }
    }
}