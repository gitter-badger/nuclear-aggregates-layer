using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10476, "Изменяем сущность OrderPosition в Dynamics CRM")]
    public class Migration10476 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration10476));
            return element;
        }
    }
}