using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class CzechLegalPersonTypeEnumCustomization : EnumCustomizationBase<LegalPersonType>, ICzechAdapted
    {
        protected override IEnumerable<LegalPersonType> GetRequiredEnumValues()
        {
            return new[]
                {
                    LegalPersonType.LegalPerson,
                    LegalPersonType.Businessman
                };
        }
    }
}