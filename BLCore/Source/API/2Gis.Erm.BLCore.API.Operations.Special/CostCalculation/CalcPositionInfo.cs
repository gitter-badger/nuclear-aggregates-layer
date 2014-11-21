using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public interface ICalcPositionInfo
    {
        long PositionId { get; set; }
        int Amount { get; set; }
    }

    [DataContract]
    public class CalcPositionInfo : ICalcPositionInfo
    {
        /// <summary>
        /// Идентификатор номенклатурной позиции.
        /// </summary>
        [DataMember]
        public long PositionId { get; set; }

        /// <summary>
        /// Количество единиц позиции
        /// </summary>
        [DataMember]
        public int Amount { get; set; }
    }
}
