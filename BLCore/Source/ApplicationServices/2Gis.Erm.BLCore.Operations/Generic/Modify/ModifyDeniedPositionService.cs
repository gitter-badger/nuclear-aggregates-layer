using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyDeniedPositionService : IModifyBusinessModelEntityService<DeniedPosition>
    {
        private readonly ICreateDeniedPositionAggregateService _createService;
        private readonly IUpdateDeniedPositionAggregateService _updateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBusinessModelEntityObtainer<DeniedPosition> _deniedPositionObtainer;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;

        public ModifyDeniedPositionService(ICreateDeniedPositionAggregateService createService,
                                           IUpdateDeniedPositionAggregateService updateService,
                                           IOperationScopeFactory operationScopeFactory,
                                           IBusinessModelEntityObtainer<DeniedPosition> deniedPositionObtainer,
                                           IPriceReadModel priceReadModel,
                                           IPositionReadModel positionReadModel,
                                           IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService)
        {
            _createService = createService;
            _updateService = updateService;
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionObtainer = deniedPositionObtainer;
            _priceReadModel = priceReadModel;
            _positionReadModel = positionReadModel;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var deniedPosition = _deniedPositionObtainer.ObtainBusinessModelEntity(domainEntityDto);
            
            if (deniedPosition.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
                {
                    Create(deniedPosition);
                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DeniedPosition>())
                {
                    Update(deniedPosition);
                    scope.Complete();
                }   
            }

            return deniedPosition.Id;
        }

        private void Update(DeniedPosition deniedPosition)
        {
            if (deniedPosition.IsSelfDenied())
            {
                _updateService.UpdateSelfDeniedPosition(deniedPosition);
            }
            else
            {
                var symmetricDeniedPosition = _getSymmetricDeniedPositionOperationService.Get(deniedPosition.Id);
                symmetricDeniedPosition.ObjectBindingType = deniedPosition.ObjectBindingType;

                _updateService.Update(deniedPosition, symmetricDeniedPosition);
            }
        }

        private void Create(DeniedPosition deniedPosition)
        {
            var existingDeniedPositions = _priceReadModel.GetDeniedPositionsOrSymmetric(deniedPosition.PositionId, deniedPosition.PositionDeniedId, deniedPosition.PriceId);
            if (existingDeniedPositions.Any())
            {
                var positionNames = _positionReadModel.GetPositionNames(existingDeniedPositions.Select(x => x.PositionId)
                                                                                               .Concat(existingDeniedPositions.Select(x => x.PositionDeniedId)));
                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                                   string.Join(",",
                                                                               existingDeniedPositions.Select(x =>
                                                                                                              string.Format("({0}, {1})",
                                                                                                                            positionNames[x.PositionId],
                                                                                                                            positionNames[x.PositionDeniedId])))));
            }

            if (deniedPosition.IsSelfDenied())
            {
                _createService.CreateSelfDeniedPosition(deniedPosition);
            }
            else
            {
                var symmetricDeniedPosition = new DeniedPosition
                                                  {
                                                      PriceId = deniedPosition.PriceId,
                                                      PositionId = deniedPosition.PositionDeniedId,
                                                      PositionDeniedId = deniedPosition.PositionId,
                                                      ObjectBindingType = deniedPosition.ObjectBindingType,
                                                      
                                                      IsActive = true
                                                  };

                _createService.Create(deniedPosition, symmetricDeniedPosition);
            }
        }
    }
}