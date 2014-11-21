using System.IO;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(14214, "Создание хранимки Integration.DeleteBuildings")]
    public class Migration14214 : TransactedMigration
    {
        private const string StoredProcedureName = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._12._16_rev._14214.DeleteBuildings.sql";

        protected override void ApplyOverride(IMigrationContext context)
        {
            ApplyScriptFromEmbeddedResource(context, StoredProcedureName);
        }

        private void ApplyScriptFromEmbeddedResource(IMigrationContext context, string resourceName)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var statement = reader.ReadToEnd();
                context.Database.ExecuteNonQuery(statement);
            }
        }
    }
}
