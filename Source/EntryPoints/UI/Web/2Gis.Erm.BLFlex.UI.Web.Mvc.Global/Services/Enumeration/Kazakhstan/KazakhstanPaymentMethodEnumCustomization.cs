using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Ukraine
{
    public class KazakhstanPaymentMethodEnumCustomization : EnumCustomizationBase<PaymentMethod>, IKazakhstanAdapted
    {
        protected override IEnumerable<PaymentMethod> GetRequiredEnumValues()
        {
            return new[]
                {
                    PaymentMethod.Undefined,
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CashPayment
                };
        }
    }
}