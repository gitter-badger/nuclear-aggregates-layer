using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public class ModifyBranchOfficeOrganizationUnitService : IModifyBusinessModelEntityService<BranchOfficeOrganizationUnit>, IChileAdapted
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> _obtainer;
        private readonly ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> _createService;
        private readonly IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> _updateService;
        private readonly ICheckInnService _checkRutService;

        public ModifyBranchOfficeOrganizationUnitService(
            IBranchOfficeReadModel readModel,
            IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> obtainer,
            ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> createService,
            IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> updateService,
            ICheckInnService checkRutService)
        {
            _readModel = readModel;
            _obtainer = obtainer;
            _createService = createService;
            _updateService = updateService;
            _checkRutService = checkRutService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            CheckRutFormat(entity);

            var dtos = entity.Parts.OfType<BranchOfficeOrganizationUnitPart>().Select(x => _readModel.GetBusinessEntityInstanceDto(x));

            if (entity.IsNew())
            {
                entity.Id = domainEntityDto.Id;
                _createService.Create(entity, dtos);
            }
            else
            {
                _updateService.Update(entity, dtos);
            }

            return entity.Id;
        }

        private void CheckRutFormat(IPartable entity)
        {
            var rut = entity.Parts.OfType<BranchOfficeOrganizationUnitPart>().Single().RepresentativeRut;

            string rutError;
            if (_checkRutService.TryGetErrorMessage(rut, out rutError))
            {
                throw new NotificationException(rutError);
            }
        }
    }
}
