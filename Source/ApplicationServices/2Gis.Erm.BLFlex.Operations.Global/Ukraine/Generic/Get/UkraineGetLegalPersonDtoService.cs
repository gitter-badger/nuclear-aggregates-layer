using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonDtoService : GetDomainEntityDtoServiceBase<LegalPerson>, IUkraineAdapted
    {
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly ISecureFinder _finder;

        public UkraineGetLegalPersonDtoService(IUserContext userContext, ILegalPersonReadModel legalPersonReadModel, ISecureFinder finder)
            : base(userContext)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _finder = finder;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            var dto = _legalPersonReadModel.GetLegalPersonDto<UkraineLegalPersonDomainEntityDto>(entityId);

            var legalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
            var entityPart = legalPerson.UkrainePart();

            dto.Egrpou = entityPart.Egrpou;
            dto.TaxationType = entityPart.TaxationType;
                
            return dto;
        }

        protected override IDomainEntityDto<LegalPerson> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new UkraineLegalPersonDomainEntityDto();
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