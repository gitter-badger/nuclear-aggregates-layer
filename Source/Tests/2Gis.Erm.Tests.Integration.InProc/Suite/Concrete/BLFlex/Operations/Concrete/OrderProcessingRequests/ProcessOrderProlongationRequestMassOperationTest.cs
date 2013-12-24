using DoubleGis.Erm.BL.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class ProcessOrderProlongationRequestMassOperationTest : IIntegrationTest
    {
        private readonly IProcessOrderProlongationRequestMassOperation _operation;

        public ProcessOrderProlongationRequestMassOperationTest(IProcessOrderProlongationRequestMassOperation operation)
        {
            _operation = operation;
        }

        public ITestResult Execute()
        {
            try
            {
                _operation.ProcessAll();
            }
            catch (BusinessLogicException)
            {
            }

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}