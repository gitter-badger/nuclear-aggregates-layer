using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4631, "Добавление кнопки 'Создание ППС' в CRM")]
    public class Migration4631 : CrmImportAndPublishCustomizationMigration
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
