using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class CzechPaymentMethodEnumCustomization : EnumCustomizationBase<PaymentMethod>, ICzechAdapted
    {
        protected override IEnumerable<PaymentMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CashPayment
                };
        }
    }
}