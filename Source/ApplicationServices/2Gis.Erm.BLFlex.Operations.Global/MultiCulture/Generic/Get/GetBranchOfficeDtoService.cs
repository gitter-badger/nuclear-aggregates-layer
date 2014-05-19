using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetBranchOfficeDtoService : GetDomainEntityDtoServiceBase<BranchOffice>, ICyprusAdapted, ICzechAdapted, IRussiaAdapted, IChileAdapted
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetBranchOfficeDtoService(IUserContext userContext, IBranchOfficeReadModel readModel, IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _readModel = readModel;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOffice> GetDto(long entityId)
        {
            return _readModel.GetBranchOfficeDto<BranchOfficeDomainEntityDto>(entityId);
        }

        protected override IDomainEntityDto<BranchOffice> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new BranchOfficeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}