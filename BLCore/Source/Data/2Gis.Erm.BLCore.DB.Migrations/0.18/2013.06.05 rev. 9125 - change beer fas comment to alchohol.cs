using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9125, "Заменяем комментарий фас пиво на алкоголь")]
    public class Migration9125 : TransactedMigration
    {
        private const string InsertStatements = @"UPDATE Billing.AdvertisementElements SET FasCommentType = 0 WHERE FasCommentType = 1";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
