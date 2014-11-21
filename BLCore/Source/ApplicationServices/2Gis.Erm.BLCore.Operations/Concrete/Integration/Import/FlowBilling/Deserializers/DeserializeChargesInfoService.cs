using System;
using System.Collections.Generic;
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
                foreach (var element in chargesElement.Elements("Charge"))
                {
                    charges.Add(CreateChargeDto(element));
                }
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
                    FirmCode = (long)element.Attribute("FirmCode"),
                    NomenclatureElementCode = (long)element.Attribute("NomenclatureElementCode"),
                    NomenclatureElementToChargeCode = (long)element.Attribute("NomenclatureElementToChargeCode"),
                    RubricCode = (long)element.Attribute("RubricCode"),
                };
        }
    }
}