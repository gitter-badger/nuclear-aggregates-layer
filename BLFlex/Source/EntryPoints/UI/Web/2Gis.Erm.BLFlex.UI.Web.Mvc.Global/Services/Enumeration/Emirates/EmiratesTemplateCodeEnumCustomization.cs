using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Emirates
{
    public class EmiratesTemplateCodeEnumCustomization : EnumCustomizationBase<TemplateCode>, IEmiratesAdapted
    {
        protected override IEnumerable<TemplateCode> GetRequiredEnumValues()
        {
            return new[]
                {
                    TemplateCode.Order,
                    TemplateCode.BillLegalPerson,
                    TemplateCode.ClientBargain,
                    TemplateCode.ClientBargainAlternativeLanguage,
                    TemplateCode.AdditionalAgreementLegalPerson,
                    TemplateCode.BargainAdditionalAgreementLegalPerson,
                    TemplateCode.AcceptanceReport,
                };
        }
    }
}