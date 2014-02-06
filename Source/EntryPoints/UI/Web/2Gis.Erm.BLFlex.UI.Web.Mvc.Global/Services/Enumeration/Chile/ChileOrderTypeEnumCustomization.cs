using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public class ChileOrderTypeEnumCustomization : EnumCustomizationBase<OrderType>, IChileAdapted
    {
        protected override IEnumerable<OrderType> GetRequiredEnumValues()
        {
            return new[]
                {
                    OrderType.Sale,
                    OrderType.SelfAds,
                    OrderType.AdsBarter,
                    OrderType.ProductBarter,
                    OrderType.ServiceBarter,
                    OrderType.SocialAds
                };
        }
    }
}