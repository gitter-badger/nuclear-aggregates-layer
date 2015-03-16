using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
	public class RussiaPhonecallPurposeEnumCustomization : EnumCustomizationBase<PhonecallPurpose>, IRussiaAdapted
    {
		protected override IEnumerable<PhonecallPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    PhonecallPurpose.NotSet,
                    PhonecallPurpose.FirstCall,
                    PhonecallPurpose.ProductPresentation,
                    PhonecallPurpose.OpportunitiesPresentation,
                    PhonecallPurpose.OfferApproval,
                    PhonecallPurpose.DecisionApproval,
                    PhonecallPurpose.Service,
                    PhonecallPurpose.Upsale,
                    PhonecallPurpose.Prolongation,                                        
                };
        }
    }
}