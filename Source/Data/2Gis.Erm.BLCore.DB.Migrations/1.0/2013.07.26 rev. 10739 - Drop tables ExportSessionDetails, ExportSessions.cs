using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10739, "Удаляем таблицы ExportSessionDetails, ExportSessions")]
    public class Migration10739 : TransactedMigration
    {
        private static readonly SchemaQualifiedObjectName[] Tables = 
        {
            ErmTableNames.ExportSessionDetails,
            ErmTableNames.ExportSessions,
        };

        protected override void ApplyOverride(IMigrationContext context)
        {
            var tables = Tables.Select(name => context.Database.GetTable(name))
                               .ToArray();

            foreach (var table in tables)
            {
                try
                {
                    table.Drop();
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("While processing {0}", table.Name), e);
                }
            }
        }
    }
}