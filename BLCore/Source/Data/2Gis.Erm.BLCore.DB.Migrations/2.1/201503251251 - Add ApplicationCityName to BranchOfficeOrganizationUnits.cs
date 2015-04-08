using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201503251251, "Добавление ApplicationCityName в BranchOfficeOrganizationUnits", "y.baranihin")]
    public class Migration201503251251 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.BranchOfficeOrganizationUnits);
            const string ColumnName = "ApplicationCityName";

            var newColumns = new[]
                                 {
                                     new InsertedColumnDefinition(7, x => new Column(x, ColumnName, DataType.NVarChar(256)) { Nullable = true }),
                                 };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}