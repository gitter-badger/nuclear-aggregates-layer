using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C
{
    [ServiceBusObjectDescription("OperationsInfo")]
    public class OperationsInfoServiceBusDto : IServiceBusDto<FlowFinancialData1C>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // FIXME {a.tukaev, 23.10.2013}: Это значение должно совпадать с кодом 1С юр. лица исполнителя, который связан с лицевым счетом, по которому происходит поступление
        // DONE {y.baranihin, 25.10.2013}: Запилил проверку 
        public string LegalEntityBranchCode1C { get; set; }
        public ICollection<OperationDto> Operations { get; set; }
    }

    public class OperationDto
    {
        public long AccountCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int OperationTypeCode { get; set; }
        public bool IsPlus { get; set; }
        public string DocumentNumber { get; set; }
    }
}