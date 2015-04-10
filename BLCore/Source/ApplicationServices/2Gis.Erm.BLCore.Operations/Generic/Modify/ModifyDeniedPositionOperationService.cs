using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyDeniedPositionOperationService : IModifyBusinessModelEntityService<DeniedPosition>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICreateOperationService<DeniedPosition> _createOperationService;
        private readonly IReplaceDeniedPositionOperationService _replaceDeniedPositionOperationService;
        private readonly IChangeDeniedPositionObjectBindingTypeOperationService _changeDeniedPositionObjectBindingTypeOperationService;

        public ModifyDeniedPositionOperationService(IOperationScopeFactory operationScopeFactory,
                                                    IPriceReadModel priceReadModel,
                                                    ICreateOperationService<DeniedPosition> createOperationService,
                                                    IReplaceDeniedPositionOperationService replaceDeniedPositionOperationService,
                                                    IChangeDeniedPositionObjectBindingTypeOperationService changeDeniedPositionObjectBindingTypeOperationService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _createOperationService = createOperationService;
            _replaceDeniedPositionOperationService = replaceDeniedPositionOperationService;
            _changeDeniedPositionObjectBindingTypeOperationService = changeDeniedPositionObjectBindingTypeOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var deniedPositionDomainEntityDto = (DeniedPositionDomainEntityDto)domainEntityDto;
            var deniedPositionId = deniedPositionDomainEntityDto.Id;

            var isPricePublishedAndActiveInfo = _priceReadModel.IsPricePublishedAndActive(deniedPositionDomainEntityDto.PriceRef.Id.Value);
            if (isPricePublishedAndActiveInfo.IsPublished)
            {
                throw new PublishedPriceModificationException(deniedPositionDomainEntityDto.IsNew()
                                                                  ? BLResources.CantCreateDeniedPositionWhenPriceIsPublished
                                                                  : BLResources.CantUpdateDeniedPositionWhenPriceIsPublished);
            }

            if (!isPricePublishedAndActiveInfo.IsActive)
            {
                throw new InactivePriceModificationException(deniedPositionDomainEntityDto.IsNew()
                                                                 ? BLResources.CantCreateDeniedPositionWhenPriceIsDeactivated
                                                                 : BLResources.CantUpdateDeniedPositionWhenPriceIsDeactivated);
            }

            if (domainEntityDto.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
                {
                    deniedPositionId = _createOperationService.Create(domainEntityDto);
                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DeniedPosition>())
                {
                    var originalDeniedPosition = _priceReadModel.GetDeniedPosition(domainEntityDto.Id);
                    if (originalDeniedPosition.PositionDeniedId != deniedPositionDomainEntityDto.PositionDeniedRef.Id.Value)
                    {
                        _replaceDeniedPositionOperationService.Replace(deniedPositionDomainEntityDto.Id, deniedPositionDomainEntityDto.PositionDeniedRef.Id.Value);
                    }

                    if (originalDeniedPosition.ObjectBindingType != deniedPositionDomainEntityDto.ObjectBindingType)
                    {
                        _changeDeniedPositionObjectBindingTypeOperationService.Change(deniedPositionDomainEntityDto.Id, deniedPositionDomainEntityDto.ObjectBindingType);
                    }

                    scope.Complete();
                }
            }

            return deniedPositionId;
        }
    }
}