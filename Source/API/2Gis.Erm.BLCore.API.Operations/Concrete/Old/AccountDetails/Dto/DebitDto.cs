using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Common.Utils.Xml;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto
{
    [DataContract]
    public sealed class DebitDto
    {
        private const string TagName = "Debit";

        public enum DebitType
        {
            None,
            Client,
            Regional
        }

        /// <summary>
        /// Стабильный идентификатор лицевого счета (r)
        /// </summary>
        [DataMember]
        public long AccountCode { get; set; }

        /// <summary>
        /// Стабильный идентификатор профиля юридического лица клиента (r)
        /// </summary>
        [DataMember]
        public long ProfileCode { get; set; }

        /// <summary>
        /// Тип заказа (r)
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }

        /// <summary>
        /// Номер заказа (r)
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Дата подписания (r)
        /// </summary>
        [DataMember]
        public DateTime SignedOnDate { get; set; }

        /// <summary>
        /// Сумма заказа (r)
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// Информация о СМИ (r)
        /// </summary>
        [DataMember]
        public string MediaInfo { get; set; }

        /// <summary>
        /// Клиентский номер БЗ, для региональных списаний (o)
        /// </summary>
        [DataMember]
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Код 1С юридического лица отделения организации (r)
        /// </summary>
        [DataMember]
        public string LegalEntityBranchCode1C { get; set; }

        public DebitType Type { get; set; }

        [DataMember]
        public IEnumerable<PlatformDistribution> PlatformDistributions { get; set; }

        public XElement ToXElement()
        {
            var elements = new object[]
                {
                    this.ToXAttribute(() => AccountCode, AccountCode),
                    this.ToXAttribute(() => LegalEntityBranchCode1C, LegalEntityBranchCode1C),
                    this.ToXAttribute(() => ProfileCode, ProfileCode),
                    this.ToXAttribute(() => OrderType, OrderType),
                    this.ToXAttribute(() => OrderNumber, OrderNumber),
                    this.ToXAttribute(() => SignedOnDate, SignedOnDate),
                    this.ToXAttribute(() => Amount, Amount),
                    this.ToXAttribute(() => MediaInfo, MediaInfo),
                    !string.IsNullOrEmpty(ClientOrderNumber) ? this.ToXAttribute(() => ClientOrderNumber, ClientOrderNumber) : null
                }
                .Concat(PlatformDistributions.Select(x => x.ToXElement()))
                .ToArray();

            return new XElement(TagName, elements);
        }

        public override string ToString()
        {
            return string.Format("AccountCode: {0}", AccountCode);
        }
    }
}