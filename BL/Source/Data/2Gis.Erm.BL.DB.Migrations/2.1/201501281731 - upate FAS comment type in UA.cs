using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations
{
    [Migration(201501281731, "Миграция существующих украинских ФАС на тип 'Новый комментарий'", "a.rechkalov")]
    public class Migration201501281731 : TransactedMigration
    {
        // Кроме как в украинских ЭРМ не должно быть украинских комментариях - поэтому не заморачиваемся
        private const string UpdateStatement = "update Billing.AdvertisementElements set FasCommentType = {0} where FasCommentType in ({1})"; 
        private const int NewFasComment = 6;
        private static readonly int[] ObsoleteUkraineFasComments = new[] { 401, 402, 403 };

        protected override void ApplyOverride(IMigrationContext context)
        {
            var statement = string.Format(UpdateStatement, NewFasComment, string.Join(", ", ObsoleteUkraineFasComments));
            context.Connection.ExecuteNonQuery(statement);
        }
    }
}