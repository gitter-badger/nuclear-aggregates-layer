using System;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Bills
{
    internal class PaymentDatePlanEvaluator
    {
        // для первого платежа - 20 число месяца, для последующих - 10 число месяца
        // Если Дата подписания > 20-го числа текущего месяца то "Дата оплаты, до" в первом (единственном) счёте устанавливать = Дате подписания БЗ.
        public DateTime Evaluate(int paymentNumber, DateTime signupDate, DateTime beginPeriod)
        {
            if (paymentNumber == 1)
            {
                var firstPaymantDate = beginPeriod.AddMonths(-1).AddDays(20 - beginPeriod.Day);
                return signupDate.Day > 20 && signupDate.Month == firstPaymantDate.Month && signupDate.Year == firstPaymantDate.Year ? signupDate : firstPaymantDate;
            }

            return beginPeriod.AddMonths(-1).AddDays(10 - beginPeriod.Day);
        }
    }
}