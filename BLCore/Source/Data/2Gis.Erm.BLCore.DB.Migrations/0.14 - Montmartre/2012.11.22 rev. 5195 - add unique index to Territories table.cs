using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5195, "Добавляем unique constraint к таблице Territories")]
    public sealed class Migration5195 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateIndex(context);
        }

        private static void CreateIndex(IMigrationContext context)
        {
            var table = context.Database.Tables["Territories", ErmSchemas.BusinessDirectory];

            var index = table.Indexes["IX_Territories_DgppId_NotNull"];
            if (index != null)
                return;

            context.Database.ExecuteNonQuery(@"
            CREATE UNIQUE NONCLUSTERED INDEX IX_Territories_DgppId_NotNull ON BusinessDirectory.Territories (DgppId) WHERE DgppId IS NOT NULL
            ");
        }
    }
}