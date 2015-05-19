using System.Xml.Linq;

using DoubleGis.Erm.BL.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201503240347, "Добавляем новый тип заказа в MS CRM", "y.baranihin")]
    public class Migration201503240347 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.customizations_201503240347);
        }
    }
}