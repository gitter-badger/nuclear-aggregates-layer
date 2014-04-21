using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Multiculture
{
    public class MultiCultureOrderTypeEnumCustomization : EnumCustomizationBase<OrderType>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
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