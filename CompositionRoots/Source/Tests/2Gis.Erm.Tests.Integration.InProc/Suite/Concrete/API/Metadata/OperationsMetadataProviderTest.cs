using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata
{
    public sealed class OperationsMetadataProviderTest : IIntegrationTest
    {
        private readonly IOperationsMetadataProvider _operationsMetadataProvider;

        public OperationsMetadataProviderTest(IOperationsMetadataProvider operationsMetadataProvider)
        {
            _operationsMetadataProvider = operationsMetadataProvider;
        }

        public ITestResult Execute()
        {
            var allOperations = _operationsMetadataProvider.GetApplicableOperations();
            var allOperationsForUser = _operationsMetadataProvider.GetApplicableOperationsForCallingUser();
            var allOperationsForContext = _operationsMetadataProvider.GetApplicableOperationsForContext(new[] { EntityType.Instance.Order() }, new[] { 29977L });

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}