using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class CzechTemplateCodeEnumCustomization : EnumCustomizationBase<TemplateCode>, ICzechAdapted
    {
        private static readonly IEnumerable<TemplateCode> IgnoredOrderTypes = new[] { TemplateCode.OrderMultiPlannedProvision };
        private static readonly IEnumerable<TemplateCode> IgnoredAdditionalAgreements = new[] { TemplateCode.FirmChangeAgreement, TemplateCode.BindingChangeAgreement,  };

        protected override IEnumerable<TemplateCode> GetRequiredEnumValues()
        {
            return Enum.GetValues(typeof(TemplateCode))
                       .Cast<TemplateCode>()
                       .Except(IgnoredOrderTypes)
                       .Except(IgnoredAdditionalAgreements);
        }
    }
}