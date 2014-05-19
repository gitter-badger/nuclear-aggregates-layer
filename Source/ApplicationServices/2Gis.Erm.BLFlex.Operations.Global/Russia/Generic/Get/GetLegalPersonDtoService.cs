using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetLegalPersonDtoService : GetDomainEntityDtoServiceBase<LegalPerson>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public GetLegalPersonDtoService(IUserContext userContext, ISecureFinder finder, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            return _legalPersonReadModel.GetLegalPersonDto<LegalPersonDomainEntityDto>(entityId);
        }

        protected override IDomainEntityDto<LegalPerson> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new LegalPersonDomainEntityDto();
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
                ? 
                new EntityReference { Id = clientId, Name = _finder.Find<Client>(x => x.Id == clientId).Select(x => x.Name).Single() }
                : new EntityReference();

            return dto;
        }
    }
}