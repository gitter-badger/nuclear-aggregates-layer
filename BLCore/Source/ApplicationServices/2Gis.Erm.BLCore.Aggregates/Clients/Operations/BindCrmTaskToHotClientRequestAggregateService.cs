using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class BindCrmTaskToHotClientRequestAggregateService : IBindCrmTaskToHotClientRequestAggregateService
    {
        private readonly IRepository<HotClientRequest> _hotClientRequestGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BindCrmTaskToHotClientRequestAggregateService(IRepository<HotClientRequest> hotClientRequestGenericRepository,
                                                             IOperationScopeFactory scopeFactory)
        {
            _hotClientRequestGenericRepository = hotClientRequestGenericRepository;
            _scopeFactory = scopeFactory;
        }

        public void BindWithCrmTask(HotClientRequest hotClientRequest, Guid taskId)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<BindCrmTaskToHotClientRequestIdentity>())
            {
                hotClientRequest.TaskId = taskId;
                _hotClientRequestGenericRepository.Update(hotClientRequest);
                _hotClientRequestGenericRepository.Save();

                operationScope.Updated(hotClientRequest)
                              .Complete();
            }
        }
    }
}