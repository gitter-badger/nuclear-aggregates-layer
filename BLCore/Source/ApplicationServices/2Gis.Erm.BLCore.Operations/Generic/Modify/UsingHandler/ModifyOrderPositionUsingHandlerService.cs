using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyOrderPositionUsingHandlerService : IModifyBusinessModelEntityService<OrderPosition>
    {
        private readonly IBusinessModelEntityObtainer<OrderPosition> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyOrderPositionUsingHandlerService(IBusinessModelEntityObtainer<OrderPosition> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderPositionDomainEntityDto)domainEntityDto;
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            _publicService.Handle(new EditOrderPositionRequest
                {
                    Entity = entity,
                    AdvertisementsLinks = dto.Advertisements.ToArray()
                });
            return entity.Id;
        }
    }
}