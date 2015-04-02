using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    public sealed class PrimaryProcessingLoadTest : IIntegrationTest
    {
        private readonly IOperationsPrimaryProcessingEnqueAggregateService _operationsPrimaryProcessingEnqueAggregateService;
        private readonly IOperationsPrimaryProcessingAbandonAggregateService _operationsPrimaryProcessingAbandonAggregateService;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;

        private readonly IReadOnlyCollection<Guid> _targetMessageFlows = new[]
                                                                    {
                                                                        new Guid("451DD85A-4BB8-42CA-87B5-76C1B580587E"),
                                                                        new Guid("DD0A14EE-3A56-41D1-AF85-6DC519A081FB"),
                                                                        new Guid("2906DF54-CCAD-4296-AF74-1C3AC6D5F99B"),
                                                                    };
        private readonly IRepository<PerformedOperationPrimaryProcessing> _primaryProcessingsRepository;
        private readonly ITracer _tracer;

        public PrimaryProcessingLoadTest(
            IRepository<PerformedOperationPrimaryProcessing> primaryProcessingsRepository,
            IOperationsPrimaryProcessingEnqueAggregateService operationsPrimaryProcessingEnqueAggregateService,
            IOperationsPrimaryProcessingAbandonAggregateService operationsPrimaryProcessingAbandonAggregateService,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService,
            ITracer tracer)
        {
            _primaryProcessingsRepository = primaryProcessingsRepository;
            _operationsPrimaryProcessingEnqueAggregateService = operationsPrimaryProcessingEnqueAggregateService;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
            _tracer = tracer;
        }

        public ITestResult Execute()
        {
            var primaryProcessings = CreatePrimaryProcessings(10000);

            _operationsPrimaryProcessingEnqueAggregateService.Push();

            _operationsPrimaryProcessingAbandonAggregateService.Abandon();

            _operationsPrimaryProcessingCompleteAggregateService.CompleteProcessing();

            _primaryProcessingsRepository.Add();
        }

        private IReadOnlyList<PerformedOperationPrimaryProcessing> CreatePrimaryProcessings(int count)
        {
            var store = new List<PerformedOperationPrimaryProcessing>(count);
            for (int i = 0; i < count; i++)
            {
                Guid useCaseId = Guid.NewGuid();
                DateTime currentTime = DateTime.UtcNow;

                store.AddRange(_targetMessageFlows.Select(flow => new PerformedOperationPrimaryProcessing
                                                                      {
                                                                          MessageFlowId = flow,
                                                                          UseCaseId = useCaseId,
                                                                          AttemptCount = 0,
                                                                          CreatedOn = currentTime
                                                                      }));
            }

            return store;
        }

        private void PushToStore(IReadOnlyCollection<PerformedOperationPrimaryProcessing> primaryProcessings)
        {
            IOperationsPrimaryProcessingEnqueAggregateService
        }
    }
}
