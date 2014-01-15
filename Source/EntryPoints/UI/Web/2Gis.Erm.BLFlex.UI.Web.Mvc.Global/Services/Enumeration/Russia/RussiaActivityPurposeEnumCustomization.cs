using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
    public class RussiaActivityPurposeEnumCustomization : EnumCustomizationBase<ActivityPurpose>, IRussiaAdapted
    {
        protected override IEnumerable<ActivityPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    ActivityPurpose.NotSet,
                    ActivityPurpose.FirstCall,
                    ActivityPurpose.ProductPresentation,
                    ActivityPurpose.OpportunitiesPresentation,
                    ActivityPurpose.OfferApproval,
                    ActivityPurpose.DecisionApproval,
                    ActivityPurpose.Prolongation,
                    ActivityPurpose.Service,
                    ActivityPurpose.Upsale
                };
        }
    }
}