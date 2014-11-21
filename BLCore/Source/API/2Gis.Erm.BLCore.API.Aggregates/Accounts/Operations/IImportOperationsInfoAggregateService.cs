using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IImportOperationsInfoAggregateService
    {
        void ImportOperationsInfos(IEnumerable<OperationsInfoServiceBusDto> operationsInfos);
    }
}