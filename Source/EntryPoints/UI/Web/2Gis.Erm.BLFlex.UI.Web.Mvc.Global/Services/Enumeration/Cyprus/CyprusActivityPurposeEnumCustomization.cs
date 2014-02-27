using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class CyprusActivityPurposeEnumCustomization : EnumCustomizationBase<ActivityPurpose>, ICyprusAdapted
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