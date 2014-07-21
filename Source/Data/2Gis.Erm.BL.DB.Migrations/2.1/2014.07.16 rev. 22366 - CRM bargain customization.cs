using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22366, "Добавление новых полей в грид Договора в CRM", "y.baranihin")]
    public class Migration22366 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Properties.Resources.customizations_22366);
        }
    }
}