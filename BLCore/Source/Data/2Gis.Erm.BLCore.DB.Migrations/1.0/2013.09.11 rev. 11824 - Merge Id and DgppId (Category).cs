using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11824, "Объединяем колонки Id и DgppId в таблице Categories")]
    public class Migration11824 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var categoriesTable = context.Database.GetTable(ErmTableNames.Categories);
            categoriesTable.RemoveForeignKey("ParentId", ErmTableNames.Categories, "Id");

            context.Database.ExecuteNonQuery(@"
UPDATE
    category
SET
    category.ParentId = parent.DgppId
FROM
    [BusinessDirectory].[Categories] category
INNER JOIN
    [BusinessDirectory].[Categories] parent
ON
    category.ParentId = parent.Id"); 
            
            // Миграция работает по аналогии с миграцией 10749
            // Основная идея - обновить столбец Id, включив каскадное обновление внешних ключей, т.о. все они будут иметь корректное значение
            // Далее, перебросить на Id индексы с DgppId и удалить последний
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryFirmAddresses, ErmTableNames.Categories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryOrganizationUnits, ErmTableNames.Categories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.OrderPositionAdvertisement, ErmTableNames.Categories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.ThemeCategories, ErmTableNames.Categories, ForeignKeyAction.Cascade);
            
            context.Database.ExecuteNonQuery("update [BusinessDirectory].[Categories] set Id = DgppId where DgppId is not null");

            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryFirmAddresses, ErmTableNames.Categories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.CategoryOrganizationUnits, ErmTableNames.Categories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.OrderPositionAdvertisement, ErmTableNames.Categories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.ThemeCategories, ErmTableNames.Categories, ForeignKeyAction.NoAction);
            
            var dgppIdColumn = categoriesTable.Columns["DgppId"];
            categoriesTable.CreateForeignKey("ParentId", ErmTableNames.Categories, "Id");

            var uqCategoriesDgppIdUniqueIndex = categoriesTable.Indexes["UQ_Categories_DgppId"];
            if (uqCategoriesDgppIdUniqueIndex != null)
            {
                uqCategoriesDgppIdUniqueIndex.Drop();
            }

            var indexesToDrop = categoriesTable.Indexes.Cast<Index>()
                                                        .Where(x => x.IndexedColumns.Contains("DgppId") && x.IndexKeyType != IndexKeyType.DriPrimaryKey)
                                                        .ToArray();

            var indexesToCreate = indexesToDrop.Select(x => EntityCopyHelper.ReplaceIndexedColumn(x, "DgppId", "Id")).ToArray();

            foreach (var index in indexesToDrop)
            {
                index.Drop();
            }

            dgppIdColumn.Drop();

            foreach (var index in indexesToCreate)
            {
                index.Create();
            }
        }
    }
}
