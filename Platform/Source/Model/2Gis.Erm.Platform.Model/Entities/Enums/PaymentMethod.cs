namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum PaymentMethod
    {
        Undefined = 0,

        // Безналичный расчет
        BankTransaction = 1,
        
        // Наличный расчет
        CashPayment = 2,

        // Банковская карта (в России кредитная/дебетовая не различаем)
        BankCard = 3,

        // Чек
        BankChequePayment = 4,

        // Дебетовая карта (в Чили различаем кредитную/дебетовую)
        DebitCard = 5,

        // Кредитная карта (в Чили различаем кредитную/дебетовую)
        CreditCard = 6,
    }
}
