using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(25404, "У нескольких комментариев ФАС не заполнено обязательное поле - исправляем", "y.baranikhin")]
    public class Migration25404 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int NewFasComment = 6;
            const int FasComment = 4;
            context.Database.ExecuteNonQuery(string.Format(@"
                update ae
                    set ae.FasCommentType = {0}
                FROM [Billing].[AdvertisementElements] ae join
	                 [Billing].[AdvertisementElementTemplates] aet 	on aet.Id = ae.AdvertisementElementTemplateId
                where aet.RestrictionType = {1} and ae.FasCommentType is null", NewFasComment, FasComment));
        }
    }
}