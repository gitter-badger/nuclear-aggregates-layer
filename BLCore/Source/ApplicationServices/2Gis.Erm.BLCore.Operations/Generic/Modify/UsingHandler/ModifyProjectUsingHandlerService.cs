using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyProjectUsingHandlerService : IModifySimplifiedModelEntityService<Project>
    {
        private readonly ISimplifiedModelEntityObtainer<Project> _projectObtainer;
        private readonly IPublicService _publicService;

        public ModifyProjectUsingHandlerService(ISimplifiedModelEntityObtainer<Project> projectObtainer, IPublicService publicService)
        {
            _projectObtainer = projectObtainer;
            _publicService = publicService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _projectObtainer.ObtainSimplifiedModelEntity(domainEntityDto);

            _publicService.Handle(new EditRequest<Project> { Entity = entity });
            return entity.Id;
        }
    }
}
