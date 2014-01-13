using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class ActivityPurposeEnumCustomization : EnumCustomizationBase<ActivityPurpose>, IRussiaAdapted
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