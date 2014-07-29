using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13170, "Настройка отображения заявок по заказам в интерфейсе MS CRM")]
    public class Migration13170 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration13170));
            return element;
        }
    }
}
