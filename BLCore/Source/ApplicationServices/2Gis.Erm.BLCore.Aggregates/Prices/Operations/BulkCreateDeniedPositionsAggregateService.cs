using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
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
            var positionDeniedPositions = deniedPositions.GroupBy(x => new
                                                                           {
                                                                               x.PositionId,
                                                                               x.PositionDeniedId,
                                                                           })
                                                         .ToDictionary(x => x.Key, y => y);

            var duplicateRules = positionDeniedPositions.Where(x => x.Value.Count() > 1);
            if (duplicateRules.Any())
            {
                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                                   string.Join(",",
                                                                               duplicateRules.Select(x =>
                                                                                                     string.Format("({0}, {1})", x.Key.PositionId, x.Key.PositionDeniedId)))));
            }

            var rulesWithoutSymmetric = positionDeniedPositions.Where(deniedPosition => !deniedPositions.Any(x =>
                                                                                                             x.PositionId == deniedPosition.Key.PositionDeniedId &&
                                                                                                             x.PositionDeniedId == deniedPosition.Key.PositionId &&
                                                                                                             x.ObjectBindingType == deniedPosition.Value.Single().ObjectBindingType));
            if (rulesWithoutSymmetric.Any())
            {
                throw new SymmetricDeniedPositionIsMissingException(string.Format(BLResources.SymmetricDeniedPositionIsMissing,
                                                                                      string.Join(",",
                                                                                                  rulesWithoutSymmetric.Select(
                                                                                                                               x =>
                                                                                                                               string.Format("({0}, {1})",
                                                                                                                                             x.Key.PositionId,
                                                                                                                                             x.Key.PositionDeniedId)))));
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