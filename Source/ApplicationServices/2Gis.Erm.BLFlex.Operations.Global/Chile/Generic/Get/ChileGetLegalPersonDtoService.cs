using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonDtoService : GetDomainEntityDtoServiceBase<LegalPerson>, IChileAdapted
    {
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IChileLegalPersonReadModel _chileLegalPersonReadModel;
        private readonly ISecureFinder _finder;

        public ChileGetLegalPersonDtoService(
            ISecureFinder finder,
            ILegalPersonReadModel legalPersonReadModel,
            IChileLegalPersonReadModel chileLegalPersonReadModel,
            IUserContext userContext)
            : base(userContext)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _chileLegalPersonReadModel = chileLegalPersonReadModel;
            _finder = finder;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            var dto = _legalPersonReadModel.GetLegalPersonDto<ChileLegalPersonDomainEntityDto>(entityId);

            var legalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
            var entityPart = legalPerson.ChilePart();

            var communeReference = _chileLegalPersonReadModel.GetCommuneReference(entityId);

            dto.OperationsKind = entityPart.OperationsKind;
            dto.CommuneRef = communeReference;

            return dto;
        }

        protected override IDomainEntityDto<LegalPerson> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new ChileLegalPersonDomainEntityDto();
            long clientId = 0;
            if (parentEntityName == EntityName.Client && parentEntityId.HasValue)
            {
                clientId = parentEntityId.Value;
            }
            else if (!string.IsNullOrEmpty(extendedInfo))
            {
                long.TryParse(Regex.Match(extendedInfo, @"ClientId=(\d+)").Groups[1].Value, out clientId);
            }

            dto.ClientRef = clientId > 0
                ? new EntityReference { Id = clientId, Name = _finder.Find<Client>(x => x.Id == clientId).Select(x => x.Name).Single() }
                : new EntityReference();

            return dto;
        }
    }
}