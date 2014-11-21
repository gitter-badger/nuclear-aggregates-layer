using System.IO;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9794, "Изменяю ImportCardsFromXml, поскольку bigint не подходит для хендлера xml")]
    public sealed class Migration9794 : TransactedMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._06._26_rev._9794.StoredProcedure";

        protected override void ApplyOverride(IMigrationContext context)
        {
            // Правка хранимых процедур, восстановление удалённых в первом пункте.
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
