using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502050722, "Добавляем индекс на граничения по моделям продаж", "y.baranihin")]
    public class Migration201502050722 : TransactedMigration
    {
        private const string ProjectIdColumnName = "ProjectId";
        private const string CategoryIdColumnName = "CategoryId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetIndexName = string.Join("_",
                                              "NCI",
                                              ErmTableNames.SalesModelCategoryRestrictions.Name,
                                              ProjectIdColumnName,
                                              CategoryIdColumnName);

            var table = context.Database.GetTable(ErmTableNames.SalesModelCategoryRestrictions);
            if (table.Indexes.Contains(targetIndexName))
            {
                return;
            }

            var index = new Index(table, targetIndexName);
            var indexColumn = new IndexedColumn(index, ProjectIdColumnName);
            index.IndexedColumns.Add(indexColumn);
            indexColumn = new IndexedColumn(index, CategoryIdColumnName);
            index.IndexedColumns.Add(indexColumn);

            index.IsClustered = false;
            index.SortInTempdb = false;
            index.OnlineIndexOperation = true;
            index.IndexKeyType = IndexKeyType.None;
            index.Create();
        }
    }
}