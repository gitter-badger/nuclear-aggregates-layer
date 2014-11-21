using System.IO;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10132, "Изменение ImportCardsFromXml для правильной обработки пустых фирм")]
    public sealed class Migration10132 : TransactedMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._07._09_rev._10132.StoredProcedure";

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
