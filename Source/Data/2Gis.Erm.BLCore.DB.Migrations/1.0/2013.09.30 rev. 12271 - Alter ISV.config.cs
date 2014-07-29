using System.IO;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12271, "Отмена предыдущих двух миграций для ISV config")]
    public sealed class Migration12271 : CrmImportAndPublishCustomizationMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._09._27_rev._12271.customizations.xml";

        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(StoredProcedurePrefix))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return XElement.Load(reader);
            } 
        }
    }
}
