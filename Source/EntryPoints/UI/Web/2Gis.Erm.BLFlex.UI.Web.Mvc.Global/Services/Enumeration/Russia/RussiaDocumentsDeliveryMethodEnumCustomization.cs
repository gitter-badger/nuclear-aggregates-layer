using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
    public class RussiaDocumentsDeliveryMethodEnumCustomization : EnumCustomizationBase<DocumentsDeliveryMethod>, IRussiaAdapted
    {
        protected override IEnumerable<DocumentsDeliveryMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    DocumentsDeliveryMethod.DeliveryByManager, 
                    DocumentsDeliveryMethod.PostOnly, 
                    DocumentsDeliveryMethod.DeliveryByClient,
                    DocumentsDeliveryMethod.ByEmail
                };
        }
    }
}