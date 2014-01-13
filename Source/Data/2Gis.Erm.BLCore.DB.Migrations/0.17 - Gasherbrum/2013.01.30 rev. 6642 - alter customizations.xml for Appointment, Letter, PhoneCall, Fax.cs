using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6642, "Меняем customizations.xml для сущностей Appointment, Letter, PhoneCall, Fax")]
    public class Migration6642 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var element = XElement.Load(new StringReader(Resources.Migration6642));
            return element;
        }
    }
}