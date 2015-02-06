using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.MultiCulture
{
    public class MultiCultureAppointmentPurposeEnumCustomization : EnumCustomizationBase<AppointmentPurpose>, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                                IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        protected override IEnumerable<AppointmentPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    AppointmentPurpose.NotSet,
                    AppointmentPurpose.FirstCall,
                    AppointmentPurpose.ProductPresentation,
                    AppointmentPurpose.Sale, // для бизнеса важно, чтобы сначала были "продажа", а затем "сервис"
                    AppointmentPurpose.Service
                };
        }
    }
}
