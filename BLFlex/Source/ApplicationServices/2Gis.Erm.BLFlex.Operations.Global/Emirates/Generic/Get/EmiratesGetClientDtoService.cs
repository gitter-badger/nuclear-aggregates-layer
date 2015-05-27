using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetClientDtoService : GetDomainEntityDtoServiceBase<Client>, IEmiratesAdapted
    {
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public EmiratesGetClientDtoService(IUserContext userContext, IClientReadModel clientReadModel, IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<Client> GetDto(long entityId)
        {
            var client = _clientReadModel.GetClient(entityId);

            var modelDto = ClientFlexSpecs.Clients.Emirates.Project.DomainEntityDto().Project(client);
            modelDto.MainFirmRef.Name = modelDto.MainFirmRef.Id.HasValue ? _firmReadModel.GetFirmName(modelDto.MainFirmRef.Id.Value) : null;
            modelDto.TerritoryRef.Name = modelDto.TerritoryRef.Id.HasValue ? _firmReadModel.GetTerritoryName(modelDto.TerritoryRef.Id.Value) : null;

            modelDto.LastDisqualifyTime = modelDto.LastDisqualifyTime.AssumeUtcKind();
            modelDto.LastQualifyTime = modelDto.LastQualifyTime.AssumeUtcKind();

            return modelDto;
        }

        protected override IDomainEntityDto<Client> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new EmiratesClientDomainEntityDto { LastQualifyTime = DateTime.UtcNow };
        }
    }
}