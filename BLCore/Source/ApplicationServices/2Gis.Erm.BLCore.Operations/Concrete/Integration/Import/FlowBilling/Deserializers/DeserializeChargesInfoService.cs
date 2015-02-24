using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Deserializers
{
    public class DeserializeChargesInfoService : IDeserializeServiceBusObjectService<ChargesInfoServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var branchCode = (long)xml.Attribute("BranchCode");
            var startDate = (DateTime)xml.Attribute("StartDate");
            var endDate = (DateTime)xml.Attribute("EndDate");

            var charges = new List<ChargeDto>();

            var chargesElement = xml.Element("Charges");
            if (chargesElement != null)
            {
                charges.AddRange(chargesElement.Elements("Charge").Select(CreateChargeDto));
            }

            return new ChargesInfoServiceBusDto
                {
                    BranchCode = branchCode,
                    StartDate = startDate,
                    EndDate = endDate,
                    Charges = charges,
                    Content = xml,
                    SessionId = Guid.NewGuid()
                };
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }

        public bool Validate(XElement xml, out string error)
        {
            // TODO {all, 21.01.2015}: надо бы уже как-нибудь сделать ресурсник для нелокализуемых строчек.
            const string AttributeIsMissingTemplate = "Отсутсвует обязательный атрибут {0}";
            var errors = new StringBuilder();
            var requiredAttributes = new[]
                                         {
                                             "BranchCode",
                                             "StartDate",
                                             "EndDate"
                                         };

            foreach (var requiredAttribute in requiredAttributes.Where(requiredAttribute => xml.Attribute(requiredAttribute) == null))
            {
                errors.AppendLine(string.Format(AttributeIsMissingTemplate, requiredAttribute));
            }

            var chargesElement = xml.Element("Charges");
            if (chargesElement != null)
            {
                var chargesWithNegativeAmount = chargesElement.Elements("Charge").Select(CreateChargeDto).Where(x => x.Amount < 0).ToArray();
                if (chargesWithNegativeAmount.Any())
                {
                    errors.AppendLine(string.Format("Can't import charges. Amount for following OrderPositions is negative: {0}",
                                                    string.Join(",", chargesWithNegativeAmount.Select(x => x.OrderPositionId.ToString()))));
                }
            }

            error = errors.ToString();
            return string.IsNullOrWhiteSpace(error);
        }

        private static ChargeDto CreateChargeDto(XElement element)
        {
            return new ChargeDto
                {
                    OrderPositionId = (long)element.Attribute("OrderPositionCode"),
                    Amount = (decimal)element.Attribute("Amount"),
                };
        }
    }
}