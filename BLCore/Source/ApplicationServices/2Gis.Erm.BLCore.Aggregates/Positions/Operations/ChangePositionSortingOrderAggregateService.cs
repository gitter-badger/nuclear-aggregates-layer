using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Position;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions.Operations
{
    public class ChangePositionSortingOrderAggregateService : IChangePositionSortingOrderAggregateService
    {
        private readonly IRepository<Position> _repository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public ChangePositionSortingOrderAggregateService(IRepository<Position> repository, IOperationScopeFactory scopeFactory, ISecurityServiceEntityAccess entityAccessService, IUserContext userContext)
        {
            _repository = repository;
            _scopeFactory = scopeFactory;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public void ChangeSorting(IEnumerable<Position> positions, IEnumerable<PositionSortingOrderDto> sorting)
        {
            var hasUpdateAccess = _entityAccessService.HasEntityAccess(EntityAccessTypes.Update, EntityName.Position, _userContext.Identity.Code, 0, 0, 0);
            if (!hasUpdateAccess)
            {
                throw new ErmSecurityException(BLResources.PositionAccessDenied);
            }

            var dataDictionary = sorting.ToDictionary(dto => dto.Id, dto => dto.Index);
            using (var scope = _scopeFactory.CreateNonCoupled<ChangePositionSortingOrderIdentity>())
            {
                foreach (var position in positions)
                {
                    position.SortingIndex = dataDictionary[position.Id];
                    _repository.Update(position);
                    scope.Updated(position);
                }

                _repository.Save();
                scope.Complete();
            }
        }
    }
}