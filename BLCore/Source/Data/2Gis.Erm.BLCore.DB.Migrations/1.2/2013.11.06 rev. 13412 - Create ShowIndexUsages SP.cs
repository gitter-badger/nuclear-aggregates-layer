using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13412, "Создание хранимой процедуры Adm.ShowIndexUsages - для получения информации об использовании индексов")]
    public sealed class Migration13412 : TransactedMigration
    {
        private const string StoredProcStatement = @"
SELECT dbs.name, d.index_id, sch.name+'.'+tbls.name, i.name, 
    d.user_seeks, d.last_user_seek,
    d.user_scans, d.last_user_scan,
    d.user_lookups, d.last_user_lookup,
    d.user_updates, d.last_user_update
FROM sys.dm_db_index_usage_stats d
INNER JOIN sys.indexes i ON
    i.object_id = d.object_id AND i.index_id = d.index_id
INNER JOIN sys.databases dbs ON dbs.database_id=d.database_id
INNER JOIN sys.tables tbls ON tbls.object_id=d.object_id
INNER JOIN sys.schemas sch ON sch.schema_id=tbls.schema_id
WHERE dbs.name='{0}' AND object_name(d.object_id, d.database_id) NOT LIKE 'sys%'";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var storedProcName = ErmStoredProcedures.ShowIndexUsages;
            if (context.Database.StoredProcedures.Contains(storedProcName.Name, storedProcName.Schema))
            {
                return;
            }

            var storedProc = new StoredProcedure(context.Database, storedProcName.Name, storedProcName.Schema)
                {
                    TextMode = false,
                    AnsiNullsStatus = true,
                    QuotedIdentifierStatus = true,
                    TextBody = string.Format(StoredProcStatement, context.Database.Name)
                };
            storedProc.Create();
        }
    }
}
