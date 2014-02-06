using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public class ChilePaymentMethodEnumCustomization : EnumCustomizationBase<PaymentMethod>, IChileAdapted
    {
        protected override IEnumerable<PaymentMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    PaymentMethod.Undefined,
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CashPayment,
                    PaymentMethod.CreditCardPayment,
                    PaymentMethod.DebitCard,
                    PaymentMethod.BankChequePayment,
                };
        }
    }
}