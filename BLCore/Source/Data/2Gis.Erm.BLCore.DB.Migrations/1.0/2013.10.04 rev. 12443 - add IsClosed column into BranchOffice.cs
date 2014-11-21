using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12443, "Добавление колонок IsClosed и ClosedOn в таблицу BranchOffices")]
    public sealed class Migration12443 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string IsClosedColumnName = "IsClosed";
            const string ClosedOnColumnName = "ClosedOn";

            var branchOfficesTable = context.Database.GetTable(ErmTableNames.BranchOffices);
            
            var newColumns = new[]
                {
                    new InsertedNotNullableColumnDefinition(
                        13, 
                        smo => new Column(smo, IsClosedColumnName, DataType.Bit) { Nullable = false },
                        IsClosedColumnName,
                        "0"),
                    new InsertedColumnDefinition(
                        13, 
                        smo => new Column(smo, ClosedOnColumnName, DataType.DateTime2(2)) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, branchOfficesTable, newColumns);
        }
    }
}
