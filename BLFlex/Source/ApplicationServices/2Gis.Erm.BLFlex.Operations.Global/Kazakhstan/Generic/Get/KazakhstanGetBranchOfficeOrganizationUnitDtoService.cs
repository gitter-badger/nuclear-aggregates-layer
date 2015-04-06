using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Get
{
    public class KazakhstanGetBranchOfficeOrganizationUnitDtoService : GetBranchOfficeOrganizationUnitDtoServiceBase<KazakhstanBranchOfficeOrganizationUnitDomainEntityDto>, IKazakhstanAdapted
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public KazakhstanGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                                   IOrganizationUnitReadModel organizationUnitReadModel,
                                                                   IBranchOfficeReadModel branchOfficeReadModel)
            : base(userContext, branchOfficeReadModel, organizationUnitReadModel)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override void SetSpecificPropertyValues(KazakhstanBranchOfficeOrganizationUnitDomainEntityDto dto)
        {
            if (dto.BranchOfficeRef.Id.HasValue)
            {
                var branchOffice = _branchOfficeReadModel.GetBranchOffice(dto.BranchOfficeRef.Id.Value);

                dto.BranchOfficeAddlId = branchOffice.Id;
                dto.BranchOfficeAddlLegalAddress = branchOffice.LegalAddress;
                dto.BranchOfficeAddlName = branchOffice.Name;
                dto.BranchOfficeAddlBin = branchOffice.Inn;
            }
        }

        protected override IProjectSpecification<BranchOfficeOrganizationUnit, KazakhstanBranchOfficeOrganizationUnitDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Kazakhstan.Project.DomainEntityDto();
        }
    }
}