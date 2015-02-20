using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.AdvModelsInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowAdvModelsInfo.Deserializers
{
    public class DeserializeAdvModelInRubricInfoService : IDeserializeServiceBusObjectService<AdvModelInRubricInfoServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var branchCode = (long)xml.Attribute("BranchCode");

            return new AdvModelInRubricInfoServiceBusDto
                       {
                           BranchCode = branchCode,
                           AdvModelInRubrics = xml.Element("AdvModelsInRubrics")
                                                  .Elements()
                                                  .Select(x =>
                                                          new AdvModelInRubricDto
                                                              {
                                                                  RubricCode = (long)x.Attribute("Code"),
                                                                  AdvModel =
                                                                      (ServiceBusSalesModel)
                                                                      Enum.Parse(typeof(ServiceBusSalesModel), x.Attribute("AdvModel").Value, true)
                                                              }).ToArray(),
                       };
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }

        public bool Validate(XElement xml, out string error)
        {
            var branchСodeAttr = xml.Attribute("BranchCode");
            if (branchСodeAttr == null)
            {
                error = "Не найден обязательный атрибут BranchCode";
                return false;
            }

            error = null;
            return true;
        }
    }
}