using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata
{
    public sealed class MetadataProviderTest : IIntegrationTest
    {
        private readonly IMetadataProvider _metadataProvider;

        public MetadataProviderTest(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public ITestResult Execute()
        {
            return OrdinaryTestResult.As.Succeeded;
        }
    }
}
