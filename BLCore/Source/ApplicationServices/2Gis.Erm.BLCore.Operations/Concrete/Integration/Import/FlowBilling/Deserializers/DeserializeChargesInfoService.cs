using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
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
            error = null;
            return true;
        }

        private static ChargeDto CreateChargeDto(XElement element)
        {
            return new ChargeDto
                {
                    OrderPositionId = (long)element.Attribute("OrderPositionCode"),
                    Amount = (long)element.Attribute("Amount"),
                };
        }
    }
}