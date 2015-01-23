using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    // TODO {all, 09.04.2014}: претендент на переезд в Core
    public class ModifyBranchOfficeService : IModifyBusinessModelEntityService<BranchOffice>
    {
        private readonly ICreateAggregateRepository<BranchOffice> _createRepository;
        private readonly IUpdateAggregateRepository<BranchOffice> _updateRepository;
        private readonly IBusinessModelEntityObtainer<BranchOffice> _obtainer;
        private readonly IPartableEntityValidator<BranchOffice> _validator;

        public ModifyBranchOfficeService(
            ICreateAggregateRepository<BranchOffice> createRepository,
            IUpdateAggregateRepository<BranchOffice> updateRepository,
            IBusinessModelEntityObtainer<BranchOffice> obtainer,
            IPartableEntityValidator<BranchOffice> validator)
        {
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _obtainer = obtainer;
            _validator = validator;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            _validator.Check(entity);

            if (entity.IsNew())
            {
                _createRepository.Create(entity);
            }
            else
            {
                _updateRepository.Update(entity);
            }

            return entity.Id;
        }
    }
}