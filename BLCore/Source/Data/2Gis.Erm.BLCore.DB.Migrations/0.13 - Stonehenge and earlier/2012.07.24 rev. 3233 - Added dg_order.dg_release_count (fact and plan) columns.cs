using System;
using System.IO;
using System.Reflection;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3233, "Обновление процедуры репликации заказа в CRM. Предварительно необходимо обновить объект 'dg_order' в CRM.")]
    public class Migration3233 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            SchemaQualifiedObjectName storedProcedureName = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrder");

            Assembly a = Assembly.GetAssembly(GetType());

            // Получить ресурс, приложенный к миграции.
            var stream = a.GetManifestResourceStream(GetType().FullName);
            if (stream == null)
                throw new InvalidOperationException("Failed to retrieve embedded resource.");
            StreamReader reader = new StreamReader(stream);
            String replicateOrderTemplate = reader.ReadToEnd();

            var spTextBody = string.Format(replicateOrderTemplate, context.CrmDatabaseName);

            try
            {
                ReplicationHelper.UpdateOrCreateReplicationSP(context, storedProcedureName, spTextBody);
            }
            catch (FailedOperationException ex)
            {
                Exception innerException;
                if (ex.RecursiveSearchMessage("\"Dg_release_count\"", out innerException))
                {
                    throw new MigrationKnownException("Не удалось применить миграцию. Вероятно, не обновлены сущности CRM.", innerException);
                }
                throw;
            }
        }
    }
}
