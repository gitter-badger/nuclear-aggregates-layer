using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23819, "Значение скидки в заказе для расширенного поиска", "a.rechkalov")]
    public class Migration23819 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.Customizations_23819);
        }
    }
}
