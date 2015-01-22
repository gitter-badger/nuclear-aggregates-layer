using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyBusinessModelEntityUsingHandlerService<TEntity> : IModifyBusinessModelEntityService<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IBusinessModelEntityObtainer<TEntity> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyBusinessModelEntityUsingHandlerService(IBusinessModelEntityObtainer<TEntity> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);
            _publicService.Handle(new EditRequest<TEntity> { Entity = entity });
            return entity.Id;
        }
    }
}