using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class CyprusDocumentsDeliveryMethodEnumCustomization : EnumCustomizationBase<DocumentsDeliveryMethod>, ICyprusAdapted
    {
        protected override IEnumerable<DocumentsDeliveryMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    DocumentsDeliveryMethod.PostOnly,
                    DocumentsDeliveryMethod.DeliveryByClient,
                    DocumentsDeliveryMethod.ByEmail,
                    DocumentsDeliveryMethod.ByCourier 
                };
        }
    }
}