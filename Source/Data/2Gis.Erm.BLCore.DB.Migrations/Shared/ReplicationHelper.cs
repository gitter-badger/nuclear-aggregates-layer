using System;
using System.IO;
using System.Reflection;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class ReplicationHelper
    {
        public static void UpdateOrCreateReplicationSP(IMigrationContext context, 
            SchemaQualifiedObjectName spName, String spTextBody)
        {
            if (context.Database.StoredProcedures.Contains(spName.Name, spName.Schema))
            {
                var sp = context.Database.StoredProcedures[spName.Name, spName.Schema];
                sp.TextBody = spTextBody;
                sp.Alter();
            }
            else
            {
                var sp = new StoredProcedure(context.Database, spName.Name, spName.Schema)
                {
                    TextMode = false,
                    AnsiNullsStatus = false,
                    QuotedIdentifierStatus = false,
                    TextBody = spTextBody
                };
                var param = new StoredProcedureParameter(sp, "@Id", DataType.Int) { DefaultValue = "NULL" };
                sp.Parameters.Add(param);

                sp.Create();
            }
        }

        /// <summary>
        /// Вытащить embedded ресурс с указанным именем.
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="embeddedResourceName"> </param>
        public static string GetAttachedResource(IMigration migration, string embeddedResourceName)
        {
            var assembly = Assembly.GetAssembly(migration.GetType());

            // Получить ресурс, приложенный к миграции.
            var resourceStream = assembly.GetManifestResourceStream(embeddedResourceName ?? migration.GetType().FullName);
            if (resourceStream == null)
            {
                throw new InvalidOperationException("Failed to retrieve embedded resource.");
            }
            var reader = new StreamReader(resourceStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Проапдейтить указанную хранимку с помощью скрипта, вытащенного из embedded ресурса с указанным именем.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="migration"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="embeddedResourceName"> </param>
        public static void UpdateStoredProcUsingAttachedTemplate(
            IMigrationContext context,
            IMigration migration,
            SchemaQualifiedObjectName storedProcedureName,
            string embeddedResourceName)
        {
            var replicateOrderTemplate = GetAttachedResource(migration, embeddedResourceName);

            var spTextBody = string.Format(replicateOrderTemplate, context.CrmDatabaseName);
            UpdateOrCreateReplicationSP(context, storedProcedureName, spTextBody);
        }
    }
}
