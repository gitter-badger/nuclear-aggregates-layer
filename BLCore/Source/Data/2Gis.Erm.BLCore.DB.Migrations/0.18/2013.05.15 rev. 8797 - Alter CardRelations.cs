using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8797, "Изменяем таблицу CardRelations")]
    public sealed class Migration8797 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.CardRelations.Name, ErmTableNames.CardRelations.Schema];
            for (int i = table.ForeignKeys.Count - 1; i >= 0; i--)
            {
                table.ForeignKeys[i].Drop();
            }
     
            var pk = table.Indexes["PK_CardRelations"];
            if (pk != null)
            {
                pk.Drop();
            }

            table.CreatePrimaryKey("Code");
        }
    }
}
