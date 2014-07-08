using System.Runtime.Serialization;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Common.Utils.Xml;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto
{
    [DataContract]
    public sealed class PlatformDistribution
    {
        private const string TagName = "PlatformDistribution";

        /// <summary>
        /// Стабильный идентификатор платформы (r)
        /// </summary>
        [DataMember]
        public PlatformEnum PlatformCode { get; set; }

        /// <summary>
        /// Сумма списания по платформе заказа (r)
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        public XElement ToXElement()
        {
            return new XElement(TagName,
                                this.ToXAttribute(() => PlatformCode, (int)PlatformCode),
                                this.ToXAttribute(() => Amount, Amount));
        }
    }
}