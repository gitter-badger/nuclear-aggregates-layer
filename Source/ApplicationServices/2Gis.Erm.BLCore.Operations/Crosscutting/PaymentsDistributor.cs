using System;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public static class PaymentsDistributor
    {
        public static decimal[] DistributePayment(int paymentsNumber, decimal totalAmount)
        {
            decimal[] payments;
            if (paymentsNumber == 0 || paymentsNumber == 1)
            {
                payments = new[] { totalAmount };
            }
            else
            {
                payments = new decimal[paymentsNumber];

                var amount = Math.Round(totalAmount / paymentsNumber, 2, MidpointRounding.ToEven);
                for (var i = 0; i < paymentsNumber - 1; i++)
                {
                    payments[i] = amount;
                }

                // last amount will correct all precision mistakes
                var lastAmount = Math.Round(totalAmount - amount * (paymentsNumber - 1), 2, MidpointRounding.ToEven);
                payments[paymentsNumber - 1] = lastAmount;
            }

            return payments;
        }
    }
}
