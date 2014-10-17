using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions
{
    public sealed class OrderValidationDiagnosticSessionFactory : IOrderValidationDiagnosticSessionFactory
    {
        private readonly IMetadataProvider _metadataProvider;

        public OrderValidationDiagnosticSessionFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IOrderValidationDiagnosticSession Create()
        {
            return new PerformanceCounterOrderValidationDiagnosticSession(_metadataProvider);
        }
    }
}