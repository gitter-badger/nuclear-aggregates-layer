using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class ValidateOrdersRequest
    {
        /// <summary>
        /// Необходимо заполнить только для проверки одного единственного заказа
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Необходимо заполнить при проверке единственного заказа
        /// </summary>
        public OrderState OrderState { get; set; }
        
        /// <summary>
        /// Необходимо заполнить для проверки по куратору
        /// </summary>
        public long? OwnerId { get; set; }

        /// <summary>
        /// Включая подчинённых куратора
        /// </summary>
        public bool IncludeOwnerDescendants { get; set; }

        /// <summary>
        /// Необходимо заполнить для проверки заказов города по выпуску
        /// </summary>
        public long? OrganizationUnitId { get; set; }

        /// <summary>
        /// Нобходимо заполнить для проверки по городу либо по куратору
        /// </summary>
        public TimePeriod Period { get; set; }

        /// <summary>
        /// Необходимо указать, запускается ли проверка в массовом режиме (перед сборкой), или в ручном (для одного Заказа)
        /// </summary>
        public ValidationType Type { get; set; }

        /// <summary>
        /// Число значащих знаков при денежных вычислениях
        /// </summary>
        public int SignificantDigitsNumber { get; set; }

        public long[] InvalidOrderIds { get; set; }
    }
}
