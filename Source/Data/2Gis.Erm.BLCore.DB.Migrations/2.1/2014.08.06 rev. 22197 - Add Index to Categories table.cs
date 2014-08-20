using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22197, "Добавление индекса в таблицу Categories", "a.rechkalov")]
    public class Migration22197 : TransactedMigration
    {
        private const string IndexName = "IX_Category_Level_IsActive_IsDeleted";

        private const string IndexQuery = "CREATE NONCLUSTERED INDEX {0} " +
                                          "ON [BusinessDirectory].[Categories] ([Level],[IsActive],[IsDeleted]) " +
                                          "INCLUDE ([Id])";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Categories);
            if (table == null || table.Indexes[IndexName] != null)
            {
                return;
            }

            var script = string.Format(IndexQuery, IndexName);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
