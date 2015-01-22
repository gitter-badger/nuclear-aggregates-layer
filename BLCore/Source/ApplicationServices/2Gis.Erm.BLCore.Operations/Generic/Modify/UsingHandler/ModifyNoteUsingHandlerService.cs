using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyNoteUsingHandlerService : IModifySimplifiedModelEntityService<Note>
    {
        private readonly ISimplifiedModelEntityObtainer<Note> _noteObtainer;
        private readonly IPublicService _publicService;

        public ModifyNoteUsingHandlerService(ISimplifiedModelEntityObtainer<Note> noteObtainer, IPublicService publicService)
        {
            _noteObtainer = noteObtainer;
            _publicService = publicService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (NoteDomainEntityDto)domainEntityDto;
            var entity = _noteObtainer.ObtainSimplifiedModelEntity(domainEntityDto);

            _publicService.Handle(new EditNoteRequest { Entity = entity, ParentTypeName = dto.ParentTypeName });
            return entity.Id;
        }
    }
}