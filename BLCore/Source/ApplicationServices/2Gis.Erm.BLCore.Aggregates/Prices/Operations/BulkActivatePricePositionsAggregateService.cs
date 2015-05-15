using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkActivatePricePositionsAggregateService : IBulkActivatePricePositionsAggregateService
    {
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkActivatePricePositionsAggregateService(IRepository<PricePosition> pricePositionGenericRepository,
                                                          IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
                                                          IRepository<AssociatedPosition> associatedPositionGenericRepository,
                                                          IOperationScopeFactory operationScopeFactory)
        {
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Activate(IEnumerable<PricePosition> pricePositions,
                            IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                            IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping)
        {
            var associatedPositionsGroupsToAactivate = new List<AssociatedPositionsGroup>();
            var associatedPositionsToActivate = new List<AssociatedPosition>();

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, PricePosition>())
            {
                foreach (var pricePosition in pricePositions)
                {
                    IEnumerable<AssociatedPositionsGroup> associatedPositionsGroups;
                    if (associatedPositionsGroupsMapping.TryGetValue(pricePosition.Id, out associatedPositionsGroups))
                    {
                        associatedPositionsGroupsToAactivate.AddRange(associatedPositionsGroups);
                    }

                    pricePosition.IsActive = true;
                    _pricePositionGenericRepository.Update(pricePosition);
                    operationScope.Updated<PricePosition>(pricePosition.Id);
                }

                var count = _pricePositionGenericRepository.Save();

                foreach (var associatedPositionsGroup in associatedPositionsGroupsToAactivate)
                {
                    IEnumerable<AssociatedPosition> associatedPositions;
                    if (associatedPositionsMapping.TryGetValue(associatedPositionsGroup.Id, out associatedPositions))
                    {
                        associatedPositionsToActivate.AddRange(associatedPositions);
                    }

                    associatedPositionsGroup.IsActive = true;
                    _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
                    operationScope.Updated<AssociatedPositionsGroup>(associatedPositionsGroup.Id);
                }

                count += _associatedPositionsGroupGenericRepository.Save();

                foreach (var associatedPosition in associatedPositionsToActivate)
                {
                    associatedPosition.IsActive = true;
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