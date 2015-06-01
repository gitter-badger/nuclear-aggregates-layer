using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateFirmContactAggregateService : IBulkCreateFirmContactAggregateService
    {
        private readonly IRepository<FirmContact> _firmContactRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateFirmContactAggregateService(IRepository<FirmContact> firmContactRepository,
                                                     IIdentityProvider identityProvider,
                                                     IOperationScopeFactory operationScopeFactory)
        {
            _firmContactRepository = firmContactRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IReadOnlyCollection<FirmContact> firmContacts)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, FirmContact>())
            {
                _identityProvider.SetFor(firmContacts);
                _firmContactRepository.AddRange(firmContacts);
                _firmContactRepository.Save();

                scope.Added(firmContacts.AsEnumerable())
                     .Complete();
            }
        }
    }
}