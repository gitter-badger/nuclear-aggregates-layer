using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class CyprusPaymentMethodEnumCustomization : EnumCustomizationBase<PaymentMethod>, ICyprusAdapted
    {
        protected override IEnumerable<PaymentMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    PaymentMethod.CashPayment,
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CreditCardPayment,
                    PaymentMethod.BankChequePayment,
                };
        }
    }
}