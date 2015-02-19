using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils.Xml;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto
{
    [DataContract]
    public sealed class DebitsInfoInitialDto
    {
        private const string TagName = "DebitsInfoInitial";

        /// <summary>
        /// Код отделения организации (r)
        /// </summary>
        [DataMember]
        public string OrganizationUnitCode { get; set; }

        /// <summary>
        /// Дата начала периода выгрузки (r)
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания периода выгрузки (r)
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Сумма клиентских списаний (r)
        /// </summary>
        [DataMember]
        public decimal ClientDebitTotalAmount { get; set; }

        [DataMember]
        public DebitDto[] Debits { get; set; }

        public XElement ToXElement()
        {
            if (string.IsNullOrWhiteSpace(OrganizationUnitCode))
            {
                throw new RequiredFieldIsEmptyException(string.Format("Не заполнен обязательный атрибут {0}", "OrganizationUnitCode"));
            }

            var innerXml = new object[]
                {
                    this.ToXAttribute(() => OrganizationUnitCode, OrganizationUnitCode),
                    this.ToXAttribute(() => StartDate, StartDate),
                    this.ToXAttribute(() => EndDate, EndDate),
                    this.ToXAttribute(() => ClientDebitTotalAmount, ClientDebitTotalAmount),
                    new XElement("ClientDebits", Debits.Where(x => x.Type == DebitDto.DebitType.Client).Select(x => x.ToXElement()))
                };

            return new XElement(TagName, innerXml);
        }
    }
}