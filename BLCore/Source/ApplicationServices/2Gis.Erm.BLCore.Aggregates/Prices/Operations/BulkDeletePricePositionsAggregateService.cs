using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkDeletePricePositionsAggregateService : IBulkDeletePricePositionsAggregateService
    {
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeletePricePositionsAggregateService(IRepository<PricePosition> pricePositionGenericRepository,
                                                        IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
                                                        IRepository<AssociatedPosition> associatedPositionGenericRepository,
                                                        IOperationScopeFactory operationScopeFactory)
        {
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Delete(IEnumerable<PricePosition> pricePositions,
                          IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                          IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping)
        {
            var associatedPositionsGroupsToDelete = new List<AssociatedPositionsGroup>();
            var associatedPositionsToDelete = new List<AssociatedPosition>();

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, PricePosition>())
            {
                foreach (var pricePosition in pricePositions)
                {
                    IEnumerable<AssociatedPositionsGroup> associatedPositionsGroups;
                    if (associatedPositionsGroupsMapping.TryGetValue(pricePosition.Id, out associatedPositionsGroups))
                    {
                        associatedPositionsGroupsToDelete.AddRange(associatedPositionsGroups);
                    }

                    _pricePositionGenericRepository.Delete(pricePosition);
                    operationScope.Deleted<PricePosition>(pricePosition.Id);
                }

                var count = _pricePositionGenericRepository.Save();

                foreach (var associatedPositionsGroup in associatedPositionsGroupsToDelete)
                {
                    IEnumerable<AssociatedPosition> associatedPositions;
                    if (associatedPositionsMapping.TryGetValue(associatedPositionsGroup.Id, out associatedPositions))
                    {
                        associatedPositionsToDelete.AddRange(associatedPositions);
                    }

                    _associatedPositionsGroupGenericRepository.Delete(associatedPositionsGroup);
                    operationScope.Deleted<AssociatedPositionsGroup>(associatedPositionsGroup.Id);
                }

                count += _associatedPositionsGroupGenericRepository.Save();

                foreach (var associatedPosition in associatedPositionsToDelete)
                {
                    _associatedPositionGenericRepository.Delete(associatedPosition);
                    operationScope.Deleted<AssociatedPosition>(associatedPosition.Id);
                }

                count += _associatedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}