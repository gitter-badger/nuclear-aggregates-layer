using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowFinancialData1C.Deserializers
{
    public sealed class DeserializeOperationsInfoService : IDeserializeServiceBusObjectService<OperationsInfoServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var operationInfo = new OperationsInfoServiceBusDto
                {
                    StartDate = (DateTime)xml.Attribute("StartDate"),
                    EndDate = (DateTime)xml.Attribute("EndDate"),
                    LegalEntityBranchCode1C = (string)xml.Attribute("LegalEntityBranchCode1C"),
                    Operations = new List<OperationDto>()
                };

            foreach (var operationElement in xml.Descendants("Operations").Single().Descendants("Operation"))
            {
                operationInfo.Operations.Add(new OperationDto
                    {
                        AccountCode = (long)operationElement.Attribute("AccountCode"),
                        Amount = (decimal)operationElement.Attribute("Amount"),
                        TransactionDate = (DateTime)operationElement.Attribute("TransactionDate"),
                        OperationTypeCode = (int)operationElement.Attribute("OperationTypeCode"),
                        DocumentNumber = (string)operationElement.Attribute("DocumentNumber"),
                        IsPlus = (bool?)operationElement.Attribute("IsPlus") ?? true
                    });
            }

            return operationInfo;
        }

        public bool Validate(XElement xml, out string error)
        {
            error = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}