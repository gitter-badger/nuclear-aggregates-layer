using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    // TODO {all, 09.04.2014}: претендент на переезд в Core
    public class ModifyBranchOfficeService : IModifyBusinessModelEntityService<BranchOffice>
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly ICreatePartableEntityAggregateService<BranchOffice, BranchOffice> _createService;
        private readonly IUpdatePartableEntityAggregateService<BranchOffice, BranchOffice> _updateService;
        private readonly IBusinessModelEntityObtainer<BranchOffice> _obtainer;
        private readonly IPartableEntityValidator<BranchOffice> _validator;

        public ModifyBranchOfficeService(IBranchOfficeReadModel readModel,
            ICreatePartableEntityAggregateService<BranchOffice, BranchOffice> createService,
            IUpdatePartableEntityAggregateService<BranchOffice, BranchOffice> updateService,
            IBusinessModelEntityObtainer<BranchOffice> obtainer,
            IPartableEntityValidator<BranchOffice> validator)
        {
            _readModel = readModel;
            _createService = createService;
            _updateService = updateService;
            _obtainer = obtainer;
            _validator = validator;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);
            
            _validator.Check(entity);

            var dtos = _readModel.GetBusinessEntityInstanceDto(entity);

            if (entity.IsNew())
            {
                _createService.Create(entity, dtos);
            }
            else
            {
                _updateService.Update(entity, dtos);
            }

            return entity.Id;
        }
    }
}
