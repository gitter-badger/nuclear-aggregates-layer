using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201502051735, "Массовое назначение для действий в CRM", "a.pashkin")]
    public class Migration201502051735 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Properties.Resources.customizations_201502051735);
        }
    }
}
