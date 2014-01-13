using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6006, "Добавление поля 'Тип ППС' в объект 'Встреча'")]
    public class Migration6006 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);
            // В customization файле нет url'ов, так что его можно заливать без обработки.
            return XElement.Load(new StringReader(attachedResource));
        }
    }
}
