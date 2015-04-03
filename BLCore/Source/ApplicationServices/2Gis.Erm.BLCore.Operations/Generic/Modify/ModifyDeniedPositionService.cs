using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
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
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;
        private readonly IDeniedPositionsDuplicatesVerifier _deniedPositionsDuplicatesVerifier;
        private readonly IPriceReadModel _priceReadModel;

        public ModifyDeniedPositionService(ICreateDeniedPositionAggregateService createService,
                                           IUpdateDeniedPositionAggregateService updateService,
                                           IOperationScopeFactory operationScopeFactory,
                                           IBusinessModelEntityObtainer<DeniedPosition> deniedPositionObtainer,
                                           IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService,
                                           IDeniedPositionsDuplicatesVerifier deniedPositionsDuplicatesVerifier,
                                           IPriceReadModel priceReadModel)
        {
            _createService = createService;
            _updateService = updateService;
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionObtainer = deniedPositionObtainer;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
            _deniedPositionsDuplicatesVerifier = deniedPositionsDuplicatesVerifier;
            _priceReadModel = priceReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var deniedPosition = _deniedPositionObtainer.ObtainBusinessModelEntity(domainEntityDto);

            var isPricePublishedAndActiveInfo = _priceReadModel.IsPricePublishedAndActive(deniedPosition.PriceId);
            if (isPricePublishedAndActiveInfo.IsPublished)
            {
                throw new PublishedPriceModificationException(deniedPosition.IsNew()
                                                                  ? BLResources.CantCreateDeniedPositionWhenPriceIsPublished
                                                                  : BLResources.CantUpdateDeniedPositionWhenPriceIsPublished);
            }

            if (!isPricePublishedAndActiveInfo.IsActive)
            {
                throw new InactiveEntityModificationException(deniedPosition.IsNew()
                                                                  ? BLResources.CantCreateDeniedPositionWhenPriceIsDeactivated
                                                                  : BLResources.CantUpdateDeniedPositionWhenPriceIsDeactivated);
            }

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
            _deniedPositionsDuplicatesVerifier.VerifyForDuplicates(deniedPosition);

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