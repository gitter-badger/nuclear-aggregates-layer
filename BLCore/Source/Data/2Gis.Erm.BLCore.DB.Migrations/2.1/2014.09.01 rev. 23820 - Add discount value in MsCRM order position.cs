using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23820, "Значение скидки в позиции заказа для расширенного поиска", "a.rechkalov")]
    public class Migration23820 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.Customizations_23820);
        }
    }
}
