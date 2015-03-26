using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
    public class RussiaTemplateCodeEnumCustomization : EnumCustomizationBase<TemplateCode>, IRussiaAdapted
    {
        protected override IEnumerable<TemplateCode> GetRequiredEnumValues()
        {
            return new[]
                       {
                           TemplateCode.BargainProlongationAgreement,
                           TemplateCode.BargainNewSalesModel,
                           TemplateCode.ClientBargain,
                           TemplateCode.AgentBargain,
                           TemplateCode.AdditionalAgreementLegalPerson,
                           TemplateCode.FirmChangeAgreement,
                           TemplateCode.BindingChangeAgreement,
                           TemplateCode.BillLegalPerson,
                           TemplateCode.JointBill,
                           TemplateCode.Order,
                           TemplateCode.OrderMultiPlannedProvision,
                           TemplateCode.TerminationNoticeLegalPerson,
                           TemplateCode.TerminationNoticeBusinessman,
                           TemplateCode.TerminationNoticeNaturalPerson,
                           TemplateCode.ReferenceInformation,
                           TemplateCode.LetterOfGuarantee,
                           TemplateCode.LetterOfGuaranteeAdvMaterial,
                       };
        }
    }
}