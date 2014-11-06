using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class UkraineFasCommentCustomization : EnumCustomizationBase<FasComment>, IUkraineAdapted
    {
        protected override IEnumerable<FasComment> GetRequiredEnumValues()
        {
            return new[]
                {
                    FasComment.UkraineDrugsAndService,
                    FasComment.UkraineAlcohol,
                    FasComment.UkraineTobaccoGoods,
                    FasComment.NewFasComment,
                };
        }
    }
}