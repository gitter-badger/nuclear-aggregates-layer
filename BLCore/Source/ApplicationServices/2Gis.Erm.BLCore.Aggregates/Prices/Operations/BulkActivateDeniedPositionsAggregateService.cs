using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkActivateDeniedPositionsAggregateService : IBulkActivateDeniedPositionsAggregateService
    {
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkActivateDeniedPositionsAggregateService(IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                           IOperationScopeFactory operationScopeFactory)
        {
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Activate(IEnumerable<DeniedPosition> deniedPositions)
        {
            CheckActivatePreconditions(deniedPositions);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    deniedPosition.IsActive = true;
                    _deniedPositionGenericRepository.Update(deniedPosition);
                    operationScope.Updated<DeniedPosition>(deniedPosition.Id);
                }

                var count = _deniedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }

        private void CheckActivatePreconditions(IEnumerable<DeniedPosition> deniedPositions)
        {
            var activeDeniedPosition = deniedPositions.FirstOrDefault(x => x.IsActive);
            if (activeDeniedPosition != null)
            {
                throw new ActiveEntityActivationException(typeof(DeniedPosition), activeDeniedPosition.Id);
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
        }
    }
}