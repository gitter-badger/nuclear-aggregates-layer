using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetBranchOfficeOrganizationUnitDtoService :
        GetBranchOfficeOrganizationUnitDtoServiceBase<EmiratesBranchOfficeOrganizationUnitDomainEntityDto>,
        IEmiratesAdapted
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public EmiratesGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                                 IBranchOfficeReadModel branchOfficeReadModel,
                                                                 IOrganizationUnitReadModel organizationUnitReadModel)
            : base(userContext, branchOfficeReadModel, organizationUnitReadModel)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override void SetSpecificPropertyValues(EmiratesBranchOfficeOrganizationUnitDomainEntityDto dto)
        {
            if (dto.BranchOfficeRef.Id.HasValue)
            {
                var branchOffice = _branchOfficeReadModel.GetBranchOffice(dto.BranchOfficeRef.Id.Value);

                dto.BranchOfficeAddlId = branchOffice.Id;
                dto.BranchOfficeAddlLegalAddress = branchOffice.LegalAddress;
                dto.BranchOfficeAddlName = branchOffice.Name;
                dto.BranchOfficeAddlCommercialLicense = branchOffice.Inn;
            }
        }

        protected override MapSpecification<BranchOfficeOrganizationUnit, EmiratesBranchOfficeOrganizationUnitDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Emirates.Project.DomainEntityDto();
        }
    }
}