using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public class ChileFasCommentCustomization : EnumCustomizationBase<FasComment>, IChileAdapted
    {
        protected override IEnumerable<FasComment> GetRequiredEnumValues()
        {
            return new[]
                {
                    FasComment.ChileAlcohol,
                    FasComment.ChileDrugsAndService,
                    FasComment.ChileMedicalReceiptDrugs,
                    FasComment.NewFasComment,
                };
        }
    }
}