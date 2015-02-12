using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412301100, "Добавление таблицы SalesModelCategoryRestrictions", "y.baranihin")]
    public class Migration201412301100 : TransactedMigration
    {
        private const string Id = "Id";
        private const string CategoryId = "CategoryId";
        private const string ProjectId = "ProjectId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetTableDescription = ErmTableNames.SalesModelCategoryRestrictions;
            var table = context.Database.Tables[targetTableDescription.Name, targetTableDescription.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableDescription.Name, targetTableDescription.Schema);

            table.CreateField(Id, DataType.BigInt, false);
            table.CreateField(CategoryId, DataType.BigInt, false);
            table.CreateField(ProjectId, DataType.BigInt, false);
            table.CreateField("SalesModel", DataType.Int, false);
            table.CreateAuditableEntityColumns();
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey(Id);
            table.CreateForeignKey(CategoryId, ErmTableNames.Categories, Id);
            table.CreateForeignKey(ProjectId, ErmTableNames.Projects, Id);
        }
    }
}
