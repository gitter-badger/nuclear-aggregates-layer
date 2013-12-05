using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata
{
    public sealed class MetadataTest : IIntegrationTest
    {
        private readonly IOperationsMetadataProvider _operationsMetadataProvider;

        public MetadataTest(IOperationsMetadataProvider operationsMetadataProvider)
        {
            _operationsMetadataProvider = operationsMetadataProvider;
        }

        public ITestResult Execute()
        {
            var allOperations = _operationsMetadataProvider.GetApplicableOperations();
            var allOperationsForUser = _operationsMetadataProvider.GetApplicableOperationsForCallingUser();
            var allOperationsForContext = _operationsMetadataProvider.GetApplicableOperationsForContext(new[] { EntityName.Order }, new[] { 29977L });

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}