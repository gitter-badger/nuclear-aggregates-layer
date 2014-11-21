using System;
using System.IO;
using System.Reflection;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3553, "Изменение хранимой процедуры createexportsession - добавлен экспорт legalperson")]
    public class Migration3553 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var storedProcedureName = ErmStoredProcedures.CreateExportSession;
            var createExportSessionSp = context.Database.StoredProcedures[storedProcedureName.Name, storedProcedureName.Schema];
            createExportSessionSp.TextBody = ExtractSPTextFromResource();
            createExportSessionSp.Alter();
        }

        private string ExtractSPTextFromResource()
        {
            Assembly a = Assembly.GetAssembly(GetType());

            // Получить ресурс, приложенный к миграции.
            using (var stream = a.GetManifestResourceStream(GetType().FullName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Failed to retrieve embedded resource.");
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
