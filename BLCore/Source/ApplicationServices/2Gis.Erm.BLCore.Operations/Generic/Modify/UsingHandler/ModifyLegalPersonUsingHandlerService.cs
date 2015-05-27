using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    // FIXME {v.lapeev, 17.02.2014}: Забрать в  BLFlex, где хендлеры для каждой бизнес-модели преобразовать в IModifyBusinessModelEntityService<LegalPerson>-сервисы
    public class ModifyLegalPersonUsingHandlerService : IModifyBusinessModelEntityService<LegalPerson>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        private readonly IBusinessModelEntityObtainer<LegalPerson> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyLegalPersonUsingHandlerService(IBusinessModelEntityObtainer<LegalPerson> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);
            _publicService.Handle(new EditRequest<LegalPerson> { Entity = entity });
            return entity.Id;
        }
    }
}