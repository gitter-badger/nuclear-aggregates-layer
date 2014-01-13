using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class CyprusOrderTypeEnumCustomization : EnumCustomizationBase<OrderType>, ICyprusAdapted
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