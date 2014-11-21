using System.IO;
using System.Xml.Linq;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(14488, "добавляем причину сделки - Личный кабинет в MS CRM", "y.baranihin")]
    public class Migration14488 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration14488));
            return element;
        }
    }
}
