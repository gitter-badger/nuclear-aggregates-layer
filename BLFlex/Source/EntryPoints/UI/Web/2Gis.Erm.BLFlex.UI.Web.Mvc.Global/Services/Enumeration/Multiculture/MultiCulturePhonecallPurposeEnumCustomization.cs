using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.MultiCulture
{
	public class MultiCulturePhonecallPurposeEnumCustomization : EnumCustomizationBase<PhonecallPurpose>, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                                IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
		protected override IEnumerable<PhonecallPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    PhonecallPurpose.NotSet,
                    PhonecallPurpose.FirstCall,
                    PhonecallPurpose.ProductPresentation,
                    PhonecallPurpose.Sale, // для бизнеса важно, чтобы сначала были "продажа", а затем "сервис"
                    PhonecallPurpose.Service
                };
        }
    }
}