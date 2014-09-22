using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.MultiCulture
{
    public class MultiCultureFasCommentCustomization : EnumCustomizationBase<FasComment>, IRussiaAdapted, ICyprusAdapted, IEmiratesAdapted
    {
        protected override IEnumerable<FasComment> GetRequiredEnumValues()
        {
            return new[]
                {
                    FasComment.Alcohol,
                    FasComment.Supplements,
                    FasComment.Drugs,
                    FasComment.DrugsAndService,
                    FasComment.NewFasComment
                };
        }
    }
}