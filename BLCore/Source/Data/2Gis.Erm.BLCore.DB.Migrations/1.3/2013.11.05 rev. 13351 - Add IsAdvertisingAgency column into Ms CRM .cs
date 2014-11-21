using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    // 2+: BL\Source\Data\2Gis.Erm.BLCore.DB.Migrations\1.2\2013.11.05 rev. 13351 - Add IsAdvertisingAgency column into Ms CRM.cs
    [Migration(13351, "создаем признак Является ли рекламным агентством клиент в MS CRM", "y.baranihin")]
    public class Migration13351 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration13351));
            return element;
        }
    }
}
