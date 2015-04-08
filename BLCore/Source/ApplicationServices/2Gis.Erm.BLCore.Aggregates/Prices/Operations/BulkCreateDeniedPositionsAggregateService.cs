using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkCreateDeniedPositionsAggregateService : IBulkCreateDeniedPositionsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateDeniedPositionsAggregateService(IIdentityProvider identityProvider,
                                                         IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                         IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IEnumerable<DeniedPosition> deniedPositions)
        {
            if (deniedPositions.Any(x => !x.IsActive || x.IsDeleted))
            {
                throw new InactiveEntityCreationException();
            }

            var duplicateRules = deniedPositions.PickDuplicates();
            if (duplicateRules.Any())
            {
                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                                   string.Join(",",
                                                                               duplicateRules.Select(x =>
                                                                                                     string.Format("({0}, {1})", x.PositionId, x.PositionDeniedId)))));
            }

            var rulesWithoutSymmetric = deniedPositions.PickRulesWithoutSymmetricOnes();
            if (rulesWithoutSymmetric.Any())
            {
                throw new SymmetricDeniedPositionIsMissingException(string.Format(BLResources.SymmetricDeniedPositionIsMissing,
                                                                                  string.Join(",",
                                                                                              rulesWithoutSymmetric.Select(x =>
                                                                                                                           string.Format("({0}, {1})",
                                                                                                                                         x.PositionId,
                                                                                                                                         x.PositionDeniedId)))));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    _identityProvider.SetFor(deniedPosition);
                    _deniedPositionGenericRepository.Add(deniedPosition);
                    operationScope.Added(deniedPosition);
                }

                _deniedPositionGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}