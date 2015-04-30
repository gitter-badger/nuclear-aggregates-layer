using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkDeactivatePricePositionsAggregateService : IBulkDeactivatePricePositionsAggregateService
    {
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeactivatePricePositionsAggregateService(IRepository<PricePosition> pricePositionGenericRepository,
                                                            IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
                                                            IRepository<AssociatedPosition> associatedPositionGenericRepository,
                                                            IOperationScopeFactory operationScopeFactory)
        {
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Deactivate(IEnumerable<PricePosition> pricePositions,
                              IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                              IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping)
        {
            var associatedPositionsGroupsToDeactivate = new List<AssociatedPositionsGroup>();
            var associatedPositionsToDeactivate = new List<AssociatedPosition>();

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, PricePosition>())
            {
                foreach (var pricePosition in pricePositions)
                {
                    IEnumerable<AssociatedPositionsGroup> associatedPositionsGroups;
                    if (associatedPositionsGroupsMapping.TryGetValue(pricePosition.Id, out associatedPositionsGroups))
                    {
                        associatedPositionsGroupsToDeactivate.AddRange(associatedPositionsGroups);
                    }

                    pricePosition.IsActive = false;
                    _pricePositionGenericRepository.Update(pricePosition);
                    operationScope.Updated<PricePosition>(pricePosition.Id);
                }

                var count = _pricePositionGenericRepository.Save();

                foreach (var associatedPositionsGroup in associatedPositionsGroupsToDeactivate)
                {
                    IEnumerable<AssociatedPosition> associatedPositions;
                    if (associatedPositionsMapping.TryGetValue(associatedPositionsGroup.Id, out associatedPositions))
                    {
                        associatedPositionsToDeactivate.AddRange(associatedPositions);
                    }

                    associatedPositionsGroup.IsActive = false;
                    _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
                    operationScope.Updated<AssociatedPositionsGroup>(associatedPositionsGroup.Id);
                }

                count += _associatedPositionsGroupGenericRepository.Save();

                foreach (var associatedPosition in associatedPositionsToDeactivate)
                {
                    associatedPosition.IsActive = false;
                    _associatedPositionGenericRepository.Update(associatedPosition);
                    operationScope.Updated<AssociatedPosition>(associatedPosition.Id);
                }

                count += _associatedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}