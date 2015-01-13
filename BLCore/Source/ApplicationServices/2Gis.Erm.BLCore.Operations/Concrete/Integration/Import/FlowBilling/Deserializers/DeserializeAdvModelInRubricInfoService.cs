﻿using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Deserializers
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
                                                  .Elements().Select(x =>
                                                                     new AdvModelInRubricDto
                                                                         {
                                                                             RubricCode = (long)x.Attribute("Code"),
                                                                             AdvModel =
                                                                                 (AdvModel)
                                                                                 Enum.Parse(typeof(AdvModel), xml.Attribute("AdvModel").Value, false)
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