using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6798, "Меняем sitemap.xml (заменяем отчёты)")]
    public class Migration6798 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);
            var element = XElement.Load(new StringReader(attachedResource));
            return element;
        }
    }
}