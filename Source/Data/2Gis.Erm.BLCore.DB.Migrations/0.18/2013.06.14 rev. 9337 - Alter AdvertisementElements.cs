using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9337, "В таблице AdvertisementElements делаем колонку FasCommentType nullable, проставляем значение null везде, где ЭРМ не является комментарием ФАС (сейчас там значение по-умолчанию - 0)")]
    public sealed class Migration9337 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            MakeFasCommentTypeNullable(context);
            RemoveUnecessaryZeroValues(context);
        }

        private static void MakeFasCommentTypeNullable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.AdvertisementElements.Name, ErmTableNames.AdvertisementElements.Schema];
            var column = table.Columns["FasCommentType"];
            if (column.Nullable)
            {
                return;
            }

            column.Nullable = true;
            column.Alter();
        }

        private static void RemoveUnecessaryZeroValues(IMigrationContext context)
        {
            const string Query = @"update Billing.AdvertisementElements set FasCommentType = null from
                                (Billing.AdvertisementElements ae
                                join Billing.AdvertisementElementTemplates aet on aet.Id = ae.AdvertisementElementTemplateId
                                ) where aet.RestrictionType != 4 and ae.FasCommentType = 0";

            context.Connection.ExecuteNonQuery(Query);
        }
    }
}