using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public class ChileTemplateCodeEnumCustomization : EnumCustomizationBase<TemplateCode>, IChileAdapted
    {
        protected override IEnumerable<TemplateCode> GetRequiredEnumValues()
        {
            return new[]
                {
                    TemplateCode.Order,
                    TemplateCode.ClientBargain,
                    TemplateCode.LetterOfGuarantee,
                    TemplateCode.BillLegalPerson,
                    TemplateCode.TerminationNoticeLegalPerson,
                };
        }
    }
}