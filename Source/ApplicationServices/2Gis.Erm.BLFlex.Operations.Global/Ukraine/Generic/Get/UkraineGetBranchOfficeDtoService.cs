using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeDtoService : GetDomainEntityDtoServiceBase<BranchOffice>, IUkraineAdapted
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public UkraineGetBranchOfficeDtoService(IUserContext userContext, IBranchOfficeReadModel readModel, IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _readModel = readModel;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOffice> GetDto(long entityId)
        {
            var dto = _readModel.GetBranchOfficeDto<UkraineBranchOfficeDomainEntityDto>(entityId);

            var brachOffice = _readModel.GetBranchOffice(entityId);
            var part = brachOffice.UkrainePart();

            dto.Ipn = part.Ipn;

            return dto;
        }

        protected override IDomainEntityDto<BranchOffice> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new UkraineBranchOfficeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}