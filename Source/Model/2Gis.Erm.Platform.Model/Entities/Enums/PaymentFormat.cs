namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum PaymentFormat
    {
        Undefined = 0,

        // Предоплата
        Prepayment = 1,

        // Рассрочка
        PaymentByInstalments = 2,

        // Отсрочка
        DeferredPayment = 3,

        // Постоплата
        Postpay = 4
    }
}
