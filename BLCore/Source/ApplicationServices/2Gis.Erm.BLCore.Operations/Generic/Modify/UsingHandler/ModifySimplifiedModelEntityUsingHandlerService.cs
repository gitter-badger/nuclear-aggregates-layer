using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifySimplifiedModelEntityUsingHandlerService<TEntity> : IModifySimplifiedModelEntityService<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly ISimplifiedModelEntityObtainer<TEntity> _simplifiedModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifySimplifiedModelEntityUsingHandlerService(ISimplifiedModelEntityObtainer<TEntity> simplifiedModelEntityObtainer, IPublicService publicService)
        {
            _simplifiedModelEntityObtainer = simplifiedModelEntityObtainer;
            _publicService = publicService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _simplifiedModelEntityObtainer.ObtainSimplifiedModelEntity(domainEntityDto);
            _publicService.Handle(new EditRequest<TEntity> { Entity = entity });
            return entity.Id;
        }
    }
}