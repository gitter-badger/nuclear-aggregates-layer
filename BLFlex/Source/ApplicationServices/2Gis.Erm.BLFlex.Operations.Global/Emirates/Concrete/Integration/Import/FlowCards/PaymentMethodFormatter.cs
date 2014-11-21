using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards
{
    // COMMENT {all, 27.05.2014}: В случае отказа от хранимки импорта карточек на остальных инсталяциях, этот класс можно будет перенести в BLCore
    public sealed class PaymentMethodFormatter : IPaymentMethodFormatter
    {
        private readonly IFirmReadModel _firmReadModel;

        public PaymentMethodFormatter(IFirmReadModel firmReadModel)
        {
            _firmReadModel = firmReadModel;
        }

        public Dictionary<long, string> FormatPaymentMethods(Dictionary<long, IEnumerable<int>> paymentMethodCodes)
        {
            var allPaymentMethodCodes = paymentMethodCodes.SelectMany(x => x.Value).Distinct().ToArray();
            var paymentMethods = _firmReadModel.GetPaymentMethods(allPaymentMethodCodes);
            var codesForUnknownPaymentMethods = allPaymentMethodCodes.Where(x => !paymentMethods.ContainsKey(x)).ToArray();
            if (codesForUnknownPaymentMethods.Any())
            {
                throw new InvalidOperationException("Не известны значения способов оплаты со следующими идентификаторами:" +
                                                    string.Join(",", codesForUnknownPaymentMethods.Select(x => x.ToString())));
            }

            var result = new Dictionary<long, string>();

            foreach (var paymentMethodCode in paymentMethodCodes)
            {
                var builder = new StringBuilder();
                for (var i = 1; i <= paymentMethodCode.Value.Count(); i++)
                {
                    builder.AppendFormat("{0}. {1} ", i, paymentMethods[paymentMethodCode.Value.ElementAt(i - 1)]);
                }

                result.Add(paymentMethodCode.Key, builder.ToString());
            }

            return result;
        }
    }
}