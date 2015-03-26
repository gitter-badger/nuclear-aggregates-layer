using System;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Bills
{
    internal class PaymentDatePlanEvaluator
    {
        // ��� ������� ������� - 20 ����� ������, ��� ����������� - 10 ����� ������
        // ���� ���� ���������� > 20-�� ����� �������� ������ �� "���� ������, ��" � ������ (������������) ����� ������������� = ���� ���������� ��.
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