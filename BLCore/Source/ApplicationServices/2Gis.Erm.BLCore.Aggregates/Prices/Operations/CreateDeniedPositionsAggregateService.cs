using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreateDeniedPositionsAggregateService : ICreateDeniedPositionsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateDeniedPositionsAggregateService(IIdentityProvider identityProvider,
                                                     IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                     IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(long priceId, long positionId, IEnumerable<DeniedPositionToCreateDto> deniedPositionDtos)
        {
            var deniedPositionsToCreate = new List<DeniedPosition>();

            foreach (var deniedPositionDto in deniedPositionDtos)
            {
                var deniedPositionToCreate = new DeniedPosition
                                                 {
                                                     PriceId = priceId,
                                                     PositionId = positionId,
                                                     PositionDeniedId = deniedPositionDto.PositionDeniedId,
                                                     ObjectBindingType = deniedPositionDto.ObjectBindingType,
                                                     IsActive = true
                                                 };

                deniedPositionsToCreate.Add(deniedPositionToCreate);

                if (!deniedPositionToCreate.IsSelfDenied())
                {
                    var symmetricDeniedPosition = deniedPositionToCreate.CreateSymmetric();
                    deniedPositionsToCreate.Add(symmetricDeniedPosition);
                }
            }


            var duplicateRules = deniedPositionsToCreate.PickDuplicates();
            if (duplicateRules.Any())
            {
                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                                   string.Join(",",
                                                                               duplicateRules.Select(x =>
                                                                                                     string.Format("({0}, {1})", x.PositionId, x.PositionDeniedId)))));
            }

            var rulesWithoutSymmetric = deniedPositionsToCreate.PickRulesWithoutSymmetricOnes();
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
                foreach (var deniedPosition in deniedPositionsToCreate)
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