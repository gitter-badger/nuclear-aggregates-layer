using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    public class ModifyBranchOfficeOrganizationUnitService : IModifyBusinessModelEntityService<BranchOfficeOrganizationUnit>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> _obtainer;
        private readonly ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> _createService;
        private readonly IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> _updateService;
        private readonly IPartableEntityValidator<BranchOfficeOrganizationUnit> _validator;

        public ModifyBranchOfficeOrganizationUnitService(
            IBranchOfficeReadModel readModel,
            IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> obtainer,
            ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> createService,
            IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> updateService,
            IPartableEntityValidator<BranchOfficeOrganizationUnit> validator)
        {
            _readModel = readModel;
            _obtainer = obtainer;
            _createService = createService;
            _updateService = updateService;
            _validator = validator;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            _validator.Check(entity);

            var dtos = _readModel.GetBusinessEntityInstanceDto(entity);

            if (entity.IsNew())
            {
                _createService.Create(entity,  dtos);
            }
            else
            {
                _updateService.Update(entity, dtos);
            }

            return entity.Id;
        }
    }
}
