using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

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
                    PaymentMethod.BankCard,
                    PaymentMethod.BankChequePayment,
                };
        }
    }
}
