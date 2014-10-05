using System.Text;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class SingleOrderValidationParams : ValidationParams
    {
        public SingleOrderValidationParams(ValidationType type) 
            : base(type, new[] { ValidationType.SingleOrderOnRegistration, ValidationType.SingleOrderOnStateChanging })
        {
        }

        /// <summary>
        /// Необходимо заполнить только для проверки одного единственного заказа
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Необходимо заполнить при проверке единственного заказа
        /// </summary>
        public OrderState CurrentOrderState { get; set; }

        /// <summary>
        /// Необходимо заполнить при проверке единственного заказа
        /// </summary>
        public OrderState NewOrderState { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("ValidationType: {0}. Token: {1}. ", Type, Token)
                .AppendFormat(
                              "OrderId: {0}. CurrentState: {1}. TargetState: {2}",
                              OrderId,
                              CurrentOrderState,
                              NewOrderState)
                .ToString();
        }
    }
}