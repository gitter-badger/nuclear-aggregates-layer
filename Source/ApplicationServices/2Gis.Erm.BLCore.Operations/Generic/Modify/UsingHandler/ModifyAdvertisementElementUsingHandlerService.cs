using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyAdvertisementElementUsingHandlerService : IModifyBusinessModelEntityService<AdvertisementElement>
    {
        private readonly IBusinessModelEntityObtainer<AdvertisementElement> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyAdvertisementElementUsingHandlerService(IBusinessModelEntityObtainer<AdvertisementElement> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        // Virtual for interception
        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            _publicService.Handle(new EditAdvertisementElementRequest
                {
                    Entity = entity,
                    FileTimestamp = dto.FileTimestamp,
                    PlainText = dto.PlainText,
                    FormattedText = dto.FormattedText
                });
            return entity.Id;
        }
    }
}