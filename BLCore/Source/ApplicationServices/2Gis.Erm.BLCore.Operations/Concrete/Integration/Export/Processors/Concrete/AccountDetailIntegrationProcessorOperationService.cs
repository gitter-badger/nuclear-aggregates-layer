using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors.Concrete
{
    public sealed class AccountDetailIntegrationProcessorOperationService : IntegrationProcessorOperationService<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>
    {
        public AccountDetailIntegrationProcessorOperationService(
            IOperationsProcessingsStoreService<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial> processingsStoreService,
            IOperationsExporter<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial> operationsExporter)
            : base(processingsStoreService, operationsExporter)
        {
        }
    }
}