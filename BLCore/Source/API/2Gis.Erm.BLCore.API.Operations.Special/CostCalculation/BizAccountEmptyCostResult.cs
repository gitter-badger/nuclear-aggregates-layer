using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    [DataContract]
    public class BizAccountEmptyCostResult : ICostCalculationResult
    {
        private const CostCalculationWcfResultType resultType = CostCalculationWcfResultType.BizAccountEmptyPositionCalcResult;

        /// <summary>
        /// Идентификатор номенклатурной позиции.
        /// </summary>
        [DataMember]
        public long PositionId { get; set; }

        [DataMember]
        public string ResultType
        {
            get { return resultType.ToString(); }
            set { }
        }
    }
}
