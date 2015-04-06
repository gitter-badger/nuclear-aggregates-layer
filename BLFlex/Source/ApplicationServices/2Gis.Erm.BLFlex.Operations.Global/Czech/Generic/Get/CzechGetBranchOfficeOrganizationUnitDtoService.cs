using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Get
{
    public class CzechGetBranchOfficeOrganizationUnitDtoService : GetBranchOfficeOrganizationUnitDtoServiceBase<CzechBranchOfficeOrganizationUnitDomainEntityDto>, ICzechAdapted
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public CzechGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                                IOrganizationUnitReadModel organizationUnitReadModel,
                                                                IBranchOfficeReadModel branchOfficeReadModel)
            : base(userContext, branchOfficeReadModel, organizationUnitReadModel)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override void SetSpecificPropertyValues(CzechBranchOfficeOrganizationUnitDomainEntityDto dto)
        {
            if (dto.BranchOfficeRef.Id.HasValue)
            {
                var branchOffice = _branchOfficeReadModel.GetBranchOffice(dto.BranchOfficeRef.Id.Value);

                dto.BranchOfficeAddlId = branchOffice.Id;
                dto.BranchOfficeAddlLegalAddress = branchOffice.LegalAddress;
                dto.BranchOfficeAddlName = branchOffice.Name;
                dto.BranchOfficeAddlDic = branchOffice.Inn;
                dto.BranchOfficeAddlIc = branchOffice.Ic;
            }
        }

        protected override IProjectSpecification<BranchOfficeOrganizationUnit, CzechBranchOfficeOrganizationUnitDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Czech.Project.DomainEntityDto();
        }
    }
}