using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
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