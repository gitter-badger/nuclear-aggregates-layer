using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502090244, "Убираем кнопки выгрузки списаний из CRM", "y.baranihin")]
    public class Migration201502090244 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.customizations_201502090244);
        }
    }
}