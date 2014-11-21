using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10749, "Объединяем колонки Id и DgppId в таблице Territories")]
    public class Migration10749 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // Основная идея - обновить столбец Id, включив каскадное обновление внешних ключей, т.о. все они будут иметь корректное значение
            // Далее, перебросить на Id индексы с DgppId и удалить последний
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Buildings, ErmTableNames.Territories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.UserTerritories, ErmTableNames.Territories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Firms, ErmTableNames.Territories, ForeignKeyAction.Cascade);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Clients, ErmTableNames.Territories, ForeignKeyAction.Cascade);

            context.Database.ExecuteNonQuery("update BusinessDirectory.Territories set Id = DgppId where DgppId is not null");

            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Buildings, ErmTableNames.Territories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.UserTerritories, ErmTableNames.Territories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Firms, ErmTableNames.Territories, ForeignKeyAction.NoAction);
            EntityCopyHelper.SetUpdateAction(context.Database, ErmTableNames.Clients, ErmTableNames.Territories, ForeignKeyAction.NoAction);


            var territoriesTable = context.Database.GetTable(ErmTableNames.Territories);
            var dgppIdColumn = territoriesTable.Columns["DgppId"];

            var ixTerritoriesDgppIdNotNull = territoriesTable.Indexes["IX_Territories_DgppId_NotNull"];
            if (ixTerritoriesDgppIdNotNull != null)
            {
                ixTerritoriesDgppIdNotNull.Drop();
            }

            var indexesToDrop = territoriesTable.Indexes.Cast<Index>()
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