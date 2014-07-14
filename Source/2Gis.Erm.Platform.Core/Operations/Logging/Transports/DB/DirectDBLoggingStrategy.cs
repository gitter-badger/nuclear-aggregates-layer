using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class DirectDBLoggingStrategy : IOperationLoggingStrategy, ISimplifiedModelConsumer
    {
        private readonly ITrackedUseCase2PerfomedBusinessOperationsConverter _trackedUseCase2PerfomedBusinessOperationsConverter;
        private readonly IRepository<PerformedBusinessOperation> _performedBusinessOperationRepository;

        public DirectDBLoggingStrategy(
            ITrackedUseCase2PerfomedBusinessOperationsConverter trackedUseCase2PerfomedBusinessOperationsConverter, 
            IRepository<PerformedBusinessOperation> performedBusinessOperationRepository)
        {
            _trackedUseCase2PerfomedBusinessOperationsConverter = trackedUseCase2PerfomedBusinessOperationsConverter;
            _performedBusinessOperationRepository = performedBusinessOperationRepository;
        }

        public bool TryLogUseCase(TrackedUseCase useCase, out string report)
        {
            report = null;
            
            var operations = _trackedUseCase2PerfomedBusinessOperationsConverter.Convert(useCase);
            _performedBusinessOperationRepository.AddRange(operations);
            _performedBusinessOperationRepository.Save();

            return true;
        }
    }
}
