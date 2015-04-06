using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201503040330, "Добавляем конопку обращения в техподдержку в MS CRM", "y.baranihin")]
    public class Migration201503040330 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.customizations_201503040330);
        }
    }
}