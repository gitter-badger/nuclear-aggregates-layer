using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyAdvertisementUsingHandlerService : IModifyBusinessModelEntityService<Advertisement>
    {
        private readonly IBusinessModelEntityObtainer<Advertisement> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyAdvertisementUsingHandlerService(IBusinessModelEntityObtainer<Advertisement> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        // Virtual for interception
        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            _publicService.Handle(new EditRequest<Advertisement>
                {
                    Entity = entity,
                });
            return entity.Id;
        }
    }
}