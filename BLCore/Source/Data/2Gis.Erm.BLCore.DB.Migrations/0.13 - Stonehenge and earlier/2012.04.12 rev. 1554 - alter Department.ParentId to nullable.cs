using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1554, "Смена колонки Department.ParentId на nullable.")]
    public class Migration_1554 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName _tableName = new SchemaQualifiedObjectName("Security", "Departments");
        private const String ColumnName = "ParentId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            Column col = context.Database.GetTable(_tableName).Columns[ColumnName];
            col.Nullable = true;
            col.Alter();
        }
    }
}
