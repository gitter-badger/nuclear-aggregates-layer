using System.IO;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12187, "Открываем окно клиента после взятия фирмы из резерва кнопкой из грида CRM")]
    public sealed class Migration12187 : CrmImportAndPublishCustomizationMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._09._27_rev._12187.customizations.xml";

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
