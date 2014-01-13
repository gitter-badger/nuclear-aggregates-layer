using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Databases.Erm.Migrations.Impl
{
    [Migration(7154, "Починка отмены звонка,встречи, писем, вот этого всего.")]
    public class Migration7154 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);
            var element = XElement.Load(new StringReader(attachedResource));
            return element;
        }
    }
}