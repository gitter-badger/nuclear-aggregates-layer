using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;
using DoubleGis.Erm.Qds.Migrations.Properties;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(21201, "Добавление кнопки Быстрый поиск заказов в MSCRM", "m.pashuk")]
    public class Migration21201 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.Migration_21201);
        }
    }
}
