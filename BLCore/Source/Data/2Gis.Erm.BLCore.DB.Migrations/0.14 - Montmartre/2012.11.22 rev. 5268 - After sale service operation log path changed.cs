using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5268, "Добавлена проверка на наличие доступа к журналу 'Журнал операций'")]
    public class Migration5268 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);

            // В приложенном sitemap.xml нет SavedQuery, т.о. использование {0} корректно.
            attachedResource = attachedResource.Replace("{0}", "Erm");

            return XElement.Load(new StringReader(attachedResource));
        }
    }
}
