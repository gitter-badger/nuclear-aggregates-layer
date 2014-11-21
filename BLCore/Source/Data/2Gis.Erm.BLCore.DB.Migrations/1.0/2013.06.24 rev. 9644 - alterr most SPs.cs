using System.IO;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9644, "Изменяю большинство хранимок, в связи с переходом на bigint")]
    public sealed class Migration9644 : TransactedMigration
    {
        private const string StoredProcedurePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._06._24_rev._9644.StoredProcedure";
        private const string TableTypePrefix = "DoubleGis.Erm.BLCore.DB.Migrations.Resources._2013._06._24_rev._9644.TableType";
        private const string DropStoredProcedures = @"
DROP PROCEDURE [Shared].[GetTemporaryTerritories]
DROP PROCEDURE [Billing].[ReplicateContacts]
DROP PROCEDURE [Integration].[ImportDepCardsFromXml]
DROP PROCEDURE [BusinessDirectory].[ReplicateFirmAddresses]
DROP PROCEDURE [BusinessDirectory].[ReplicateFirms]
DROP PROCEDURE [BusinessDirectory].[ReplicateTerritories]";

        protected override void ApplyOverride(IMigrationContext context)
        {
            // Некоторые хранимки требуется удалить, чтобы провести манипуляции с существующими табличными типами данных. Позже эти хранимки будут восстановлены.
            context.Connection.ExecuteNonQuery(DropStoredProcedures);

            // Правка табличных типов данных.
            ApplyScriptsFromEmbeddedResource(context, TableTypePrefix);

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
