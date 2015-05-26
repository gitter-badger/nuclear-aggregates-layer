﻿using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Cyprus;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Get
{
    public class CyprusGetBranchOfficeOrganizationUnitDtoService : GetBranchOfficeOrganizationUnitDtoServiceBase<CyprusBranchOfficeOrganizationUnitDomainEntityDto>, ICyprusAdapted
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public CyprusGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                              IBranchOfficeReadModel branchOfficeReadModel,
                                                              IOrganizationUnitReadModel organizationUnitReadModel)
            : base(userContext, branchOfficeReadModel, organizationUnitReadModel)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override void SetSpecificPropertyValues(CyprusBranchOfficeOrganizationUnitDomainEntityDto dto)
        {
            if (dto.BranchOfficeRef.Id.HasValue)
            {
                var branchOffice = _branchOfficeReadModel.GetBranchOffice(dto.BranchOfficeRef.Id.Value);

                dto.BranchOfficeAddlId = branchOffice.Id;
                dto.BranchOfficeAddlLegalAddress = branchOffice.LegalAddress;
                dto.BranchOfficeAddlName = branchOffice.Name;
                dto.BranchOfficeAddlTic = branchOffice.Inn;
        }
                    }

        protected override ProjectSpecification<BranchOfficeOrganizationUnit, CyprusBranchOfficeOrganizationUnitDomainEntityDto> GetProjectSpecification()
                    {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Cyprus.Project.DomainEntityDto();
        }
    }
}