using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    /// <summary>
    /// Тип проверки заказа
    /// </summary>
    [DataContract]
    public enum ValidationType
    {
        /// <summary>
        /// Проверка конкретного заказа перед переводом его в статус "На утверждении"
        /// </summary>
        [EnumMember]
        SingleOrderOnRegistration = 1,

        /// <summary>
        /// Проверка заказов на готовность к технической сборке
        /// </summary>
        [EnumMember]
        PreReleaseBeta = 2,

        /// <summary>
        /// Проверка заказов на готовность к финальной сборке
        /// </summary>
        [EnumMember]
        PreReleaseFinal = 4,

        /// <summary>
        /// Проверка заказов на готовность к сборке, запускаемая через UI. Результат - отчет об ошибках
        /// </summary>
        [EnumMember]
        ManualReport = 8,

        /// <summary>
        /// Проверка заказов на готовность к сборке с проверкой на деньги.
        /// </summary>
        [EnumMember]
        ManualReportWithAccountsCheck = 16,
        
        /// <summary>
        /// Проверка конкретного заказа при смене статуса
        /// </summary>
        [EnumMember]
        SingleOrderOnStateChanging = 32
    }
}