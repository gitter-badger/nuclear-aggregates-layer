using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class CzechFasCommentCustomization : EnumCustomizationBase<FasComment>, ICzechAdapted
    {
        protected override IEnumerable<FasComment> GetRequiredEnumValues()
        {
            return new[]
                {
                    FasComment.NewFasComment,
                    FasComment.AlcoholAdvertising,
                    FasComment.MedsMultiple,
                    FasComment.MedsSingle,
                    FasComment.DietarySupplement,
                    FasComment.SpecialNutrition,
                    FasComment.ChildNutrition,
                    FasComment.FinancilaServices,
                    FasComment.MedsTraditional,
                    FasComment.Biocides
                };
        }
    }
}