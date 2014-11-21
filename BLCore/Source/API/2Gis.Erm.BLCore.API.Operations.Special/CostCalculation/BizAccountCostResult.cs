using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    [DataContract]
    public class BizAccountPositionCostResult : ICostCalculationResult
    {
        private const CostCalculationWcfResultType resultType = CostCalculationWcfResultType.BizAccountPositionCalcResult;

        /// <summary>
        /// Идентификатор номенклатурной позиции.
        /// </summary>
        [DataMember]
        public long PositionId { get; set; }

        /// <summary>
        /// Планируемая сумма к оплате.
        /// </summary>
        [DataMember]
        public decimal PayablePlan { get; set; }

        /// <summary>
        /// Примененная скидка в денежном значении.
        /// </summary>
        [DataMember]
        public decimal DiscountSum { get; set; }

        [DataMember]
        public string ResultType
        {
            get { return resultType.ToString(); }
            set { }
        }
    }
}
