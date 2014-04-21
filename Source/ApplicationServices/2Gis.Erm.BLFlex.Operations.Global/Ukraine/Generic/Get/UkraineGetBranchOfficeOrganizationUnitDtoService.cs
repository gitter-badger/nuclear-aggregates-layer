using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<BranchOfficeOrganizationUnit>, IUkraineAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public UkraineGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext,
                                                                ISecureFinder finder,
                                                                IBranchOfficeReadModel readModel,
                                                                IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _finder = finder;
            _readModel = readModel;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> GetDto(long entityId)
        {
            var dto = _readModel.GetBranchOfficeOrganizationUnitDto<UkraineBranchOfficeOrganizationUnitDomainEntityDto>(entityId);

            var branchOffice = _readModel.GetBranchOffice(dto.BranchOfficeAddlId);
            var part = branchOffice.UkrainePart();

            dto.BranchOfficeAddlIpn = part.Ipn;
            return dto;
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new UkraineBranchOfficeOrganizationUnitDomainEntityDto
            {
                IdentityServiceUrl = _identityServiceSettings.RestUrl
            };

            switch (parentEntityName)
            {
                case EntityName.BranchOffice:
                    var branchOffice = _readModel.GetBranchOffice(parentEntityId.Value);

                    dto.BranchOfficeRef = new EntityReference { Id = parentEntityId.Value, Name = branchOffice.Name };
                    dto.ShortLegalName = dto.BranchOfficeRef.Name;

                    break;
                case EntityName.OrganizationUnit:
                    dto.OrganizationUnitRef = new EntityReference { Id = parentEntityId.Value, Name = _finder.Find<OrganizationUnit>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault() };
                    
                    break;
            }

            return dto;
        }
    }
}