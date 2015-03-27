using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class CzechTemplateCodeEnumCustomization : EnumCustomizationBase<TemplateCode>, ICzechAdapted
    {
        protected override IEnumerable<TemplateCode> GetRequiredEnumValues()
        {
            return new[]
                       {
                           TemplateCode.ClientBargain,
                           TemplateCode.AdditionalAgreementLegalPerson,
                           TemplateCode.AdditionalAgreementBusinessman,
                           TemplateCode.BargainAdditionalAgreementLegalPerson,
                           TemplateCode.BargainAdditionalAgreementBusinessman,
                           TemplateCode.BillLegalPerson,
                           TemplateCode.BillBusinessman,
                           TemplateCode.Order,
                           TemplateCode.TerminationNoticeLegalPerson,
                           TemplateCode.TerminationNoticeBusinessman,
                           TemplateCode.TerminationNoticeWithoutReasonLegalPerson,
                           TemplateCode.TerminationNoticeWithoutReasonBusinessman,
                           TemplateCode.TerminationNoticeBargainLegalPerson,
                           TemplateCode.TerminationNoticeBargainBusinessman,
                           TemplateCode.TerminationNoticeBargainWithoutReasonLegalPerson,
                           TemplateCode.TerminationNoticeBargainWithoutReasonBusinessman,
                           TemplateCode.LetterOfGuarantee,
                       };
        }
    }
}