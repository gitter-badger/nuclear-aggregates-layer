using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    /// <summary>
    /// Результаты расчета. Данные, которые мы сохраняем в таблицу OrderPositions
    /// + явно выделенный Vat
    /// + разложение по вложенным позициям 
    /// </summary>
    [DataContract]
    public class CalculationResult
    {
        /// <summary>
        /// Идентификатор номенклатурной позиции.
        /// </summary>
        [DataMember]
        public long PositionId { get; set; }

        /// <summary>
        /// Идентификатор прайса.
        /// </summary>
        [DataMember]
        public long PriceId { get; set; }

        /// <summary>
        /// Коэфициент группы рубрик, который применяется к данной позиции.
        /// </summary>
        [DataMember]
        public decimal Rate { get; set; }

        /// <summary>
        /// Планируемая сумма к оплате.
        /// </summary>
        [DataMember]
        public decimal PayablePlan { get; set; }

        /// <summary>
        /// Сумма к оплате без учета НДС.
        /// </summary>
        [DataMember]
        public decimal PayablePlanWoVat { get; set; }

        /// <summary>
        /// Цена за единицу.
        /// </summary>
        [DataMember]
        public decimal PricePerUnit { get; set; }

        /// <summary>
        /// Цена за единицу с НДС.
        /// </summary>
        [DataMember]
        public decimal PricePerUnitWithVat { get; set; }

        [DataMember]
        public decimal PayablePriceWithoutVat { get; set; }

        [DataMember]
        public decimal PayablePriceWithVat { get; set; }

        /// <summary>
        /// Примененная скидка в денежном значении.
        /// </summary>
        [DataMember]
        public decimal DiscountSum { get; set; }

        /// <summary>
        /// Примененная скидка в процентах.
        /// </summary>
        [DataMember]
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Планируемое количество к отгрузке.
        /// </summary>
        [DataMember]
        public int ShipmentPlan { get; set; }

        /// <summary>
        /// Значение НДС, которое показываем клиенту.
        /// </summary>
        [DataMember]
        public decimal? Vat { get; set; }

        /// <summary>
        /// Разложение по подпозициям.
        /// </summary>
        [DataMember]
        public IList<CalculationResult> PositionCalcs { get; set; }
    }
}
