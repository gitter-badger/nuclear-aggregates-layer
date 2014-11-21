using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public class ChilePaymentMethodEnumCustomization : EnumCustomizationBase<PaymentMethod>, IChileAdapted
    {
        protected override IEnumerable<PaymentMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CashPayment,
                    PaymentMethod.CreditCard,
                    PaymentMethod.DebitCard,
                    PaymentMethod.BankChequePayment,
                };
        }
    }
}