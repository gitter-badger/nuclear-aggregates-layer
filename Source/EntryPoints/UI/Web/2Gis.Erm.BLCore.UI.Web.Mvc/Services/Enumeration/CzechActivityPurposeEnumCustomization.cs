using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class CzechActivityPurposeEnumCustomization : EnumCustomizationBase<ActivityPurpose>, ICzechAdapted
    {
        protected override IEnumerable<ActivityPurpose> GetRequiredEnumValues()
        {
            return new[]
                {
                    ActivityPurpose.NotSet,
                    ActivityPurpose.FirstCall,
                    ActivityPurpose.ProductPresentation,
                    ActivityPurpose.Sale, // для бизнеса важно, чтобы сначала были "продажа", а затем "сервис"
                    ActivityPurpose.Service
                };
        }
    }
}