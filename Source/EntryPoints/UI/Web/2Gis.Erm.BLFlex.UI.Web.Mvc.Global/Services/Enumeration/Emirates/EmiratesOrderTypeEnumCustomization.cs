using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Emirates
{
    public class EmiratesOrderTypeEnumCustomization : EnumCustomizationBase<OrderType>, IEmiratesAdapted
    {
        protected override IEnumerable<OrderType> GetRequiredEnumValues()
        {
            return new[]
                {
                    OrderType.Sale,
                    OrderType.SelfAds,
                    OrderType.SocialAds
                };
        }
    }
}