﻿using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class RussiaGetBranchOfficeOrganizationUnitDtoService : GetBranchOfficeOrganizationUnitDtoServiceBase<RussiaBranchOfficeOrganizationUnitDomainEntityDto>, IRussiaAdapted
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public RussiaGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                                IOrganizationUnitReadModel organizationUnitReadModel,
                                                                IBranchOfficeReadModel branchOfficeReadModel,
                                                                IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext, branchOfficeReadModel, organizationUnitReadModel, identityServiceSettings)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override void SetSpecificPropertyValues(RussiaBranchOfficeOrganizationUnitDomainEntityDto dto)
        {
            if (dto.BranchOfficeRef.Id.HasValue)
            {
                var branchOffice = _branchOfficeReadModel.GetBranchOffice(dto.BranchOfficeRef.Id.Value);

                dto.BranchOfficeAddlId = branchOffice.Id;
                dto.BranchOfficeAddlLegalAddress = branchOffice.LegalAddress;
                dto.BranchOfficeAddlName = branchOffice.Name;
                dto.BranchOfficeAddlInn = branchOffice.Inn;
            }
        }

        protected override IProjectSpecification<BranchOfficeOrganizationUnit, RussiaBranchOfficeOrganizationUnitDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Russia.Project.DomainEntityDto();
        }
    }
}