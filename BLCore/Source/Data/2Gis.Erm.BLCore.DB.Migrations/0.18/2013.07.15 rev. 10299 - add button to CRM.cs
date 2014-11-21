using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10299, "Добавляем кнопочку для поиска заказов с заглушками")]
    public class Migration10299 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);
            var element = XElement.Load(new StringReader(attachedResource));
            return element;
        }
    }
}
