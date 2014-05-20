using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20377, "Правки в ISV.Config для поддержки cross domain modal dialog - поддержка массовых операций ERM-3992", "i.maslennikov")]
    public class Migration20377 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            return XElement.Parse(Resources.Customizations_20377);
        }
    }
}
