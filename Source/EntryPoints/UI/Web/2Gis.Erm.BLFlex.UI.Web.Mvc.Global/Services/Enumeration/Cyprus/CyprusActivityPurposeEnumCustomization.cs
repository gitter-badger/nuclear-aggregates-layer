using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

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