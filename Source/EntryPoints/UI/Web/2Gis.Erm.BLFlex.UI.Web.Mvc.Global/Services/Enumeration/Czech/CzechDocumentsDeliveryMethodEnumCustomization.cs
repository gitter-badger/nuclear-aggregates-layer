using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Czech
{
    public class CzechDocumentsDeliveryMethodEnumCustomization : EnumCustomizationBase<DocumentsDeliveryMethod>, ICzechAdapted
    {
        protected override IEnumerable<DocumentsDeliveryMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    DocumentsDeliveryMethod.Undefined,
                    DocumentsDeliveryMethod.DeliveryByClient,
                    DocumentsDeliveryMethod.ByEmail,
                    DocumentsDeliveryMethod.ByCourier
                };
        }
    }
}