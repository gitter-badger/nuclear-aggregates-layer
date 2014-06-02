using System;

using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public sealed class PaymentsDistributor : IPaymentsDistributor
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public PaymentsDistributor(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        public decimal[] DistributePayment(int paymentsNumber, decimal totalAmount)
        {
            decimal[] payments;
            if (paymentsNumber == 0 || paymentsNumber == 1)
            {
                payments = new[] { totalAmount };
            }
            else
            {
                payments = new decimal[paymentsNumber];

                var amount = Math.Round(totalAmount / paymentsNumber, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                for (var i = 0; i < paymentsNumber - 1; i++)
                {
                    payments[i] = amount;
                }

                // last amount will correct all precision mistakes
                var lastAmount = Math.Round(totalAmount - (amount * (paymentsNumber - 1)), _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                payments[paymentsNumber - 1] = lastAmount;
            }

            return payments;
        }
    }
}
