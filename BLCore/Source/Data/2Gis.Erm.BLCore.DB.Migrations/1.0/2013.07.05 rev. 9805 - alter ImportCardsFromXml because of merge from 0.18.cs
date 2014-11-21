using System.IO;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9805, "Изменяю ImportCardsFromXml, она была изменена в 0.18, но без учёта bigint")]
    public sealed class Migration9805 : TransactedMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._07._05_rev._9805.StoredProcedure";

        protected override void ApplyOverride(IMigrationContext context)
        {
            ApplyScriptsFromEmbeddedResource(context, StoredProcedurePrefix);
        }

        private void ApplyScriptsFromEmbeddedResource(IMigrationContext context, string prefix)
        {
            var names = GetType().Assembly.GetManifestResourceNames().Where(name => name.StartsWith(prefix));
            foreach (var name in names)
            {
                ApplyScriptFromEmbeddedResource(context, name);
            }
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
