using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BL.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BL.DB.Migrations.bb_s2
{
    [Migration(15501, "меняем URL слияния юр. лиц", "y.baranihin")]
    public class Migration15501 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration15501));
            return element;
        }
    }
}
