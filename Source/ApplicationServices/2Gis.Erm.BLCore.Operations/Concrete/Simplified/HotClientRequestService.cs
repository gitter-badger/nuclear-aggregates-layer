using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public class HotClientRequestService : IHotClientRequestService
    {
        private readonly IRepository<HotClientRequest> _hotClientRequestGenericRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public HotClientRequestService(
            IRepository<HotClientRequest> hotClientRequestGenericRepository,
            IFinder finder,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _hotClientRequestGenericRepository = hotClientRequestGenericRepository;
            _finder = finder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public int CreateOrUpdate(HotClientRequest request)
        {
            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(request))
            {
                _identityProvider.SetFor(request);
                _hotClientRequestGenericRepository.Add(request);

                var result = _hotClientRequestGenericRepository.Save();

                operationScope.Added<HotClientRequest>(request.Id)
                              .Complete();

                return result;
            }
        }

        public HotClientRequest GetHotClientRequest(long id)
        {
            return _finder.Find(Specs.Find.ById<HotClientRequest>(id)).Single();
        }

        public void LinkWithCrmTask(long id, Guid taskId)
        {
            var hotClient = _finder.Find(Specs.Find.ById<HotClientRequest>(id)).Single();

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(hotClient))
            {
                hotClient.TaskId = taskId;
                _hotClientRequestGenericRepository.Update(hotClient);
                _hotClientRequestGenericRepository.Save();

                operationScope.Updated<HotClientRequest>(hotClient.Id)
                              .Complete();
            }
        }
    }
}
