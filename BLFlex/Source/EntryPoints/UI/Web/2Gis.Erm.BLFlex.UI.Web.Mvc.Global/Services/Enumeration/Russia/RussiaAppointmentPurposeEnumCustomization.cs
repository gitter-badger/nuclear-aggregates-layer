using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
    public class RussiaAppointmentPurposeEnumCustomization : EnumCustomizationBase<AppointmentPurpose>, IRussiaAdapted
    {
        protected override IEnumerable<AppointmentPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    AppointmentPurpose.NotSet,
                    AppointmentPurpose.ProductPresentation,
                    AppointmentPurpose.OpportunitiesPresentation,
                    AppointmentPurpose.OfferApproval,
                    AppointmentPurpose.DecisionApproval,
                    AppointmentPurpose.Service,
                    AppointmentPurpose.Upsale,
                    AppointmentPurpose.Prolongation,                                        
                };
        }
    }
}
