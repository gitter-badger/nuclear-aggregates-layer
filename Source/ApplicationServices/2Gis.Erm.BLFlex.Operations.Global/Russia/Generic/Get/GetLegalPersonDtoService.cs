using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetLegalPersonDtoService : GetDomainEntityDtoServiceBase<LegalPerson>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;

        public GetLegalPersonDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            var modelDto = _finder.Find<LegalPerson>(x => x.Id == entityId)
                                  .Select(entity => new LegalPersonDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          LegalName = entity.LegalName,
                                          ShortName = entity.ShortName,
                                          LegalPersonTypeEnum = (LegalPersonType)entity.LegalPersonTypeEnum,
                                          LegalAddress = entity.LegalAddress,
                                          Inn = (LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson ? entity.Inn : null,
                                          Kpp = entity.Kpp,
                                          VAT = entity.VAT,
                                          BusinessmanInn = (LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.Businessman ? entity.Inn : null,
                                          PassportSeries = entity.PassportSeries,
                                          PassportNumber = entity.PassportNumber,
                                          PassportIssuedBy = entity.PassportIssuedBy,
                                          RegistrationAddress = entity.RegistrationAddress,
                                          ClientRef = new EntityReference { Id = entity.ClientId, Name = entity.Client.Name },
                                          IsInSyncWith1C = entity.IsInSyncWith1C,
                                          ReplicationCode = entity.ReplicationCode,
                                          Comment = entity.Comment,
                                          OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                          CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                          CreatedOn = entity.CreatedOn,
                                          IsActive = entity.IsActive,
                                          IsDeleted = entity.IsDeleted,
                                          ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                          ModifiedOn = entity.ModifiedOn,
                                          HasProfiles = entity.LegalPersonProfiles.Any(),
                                          Timestamp = entity.Timestamp
                                      })
                                  .Single();
            return modelDto;
        }

        protected override IDomainEntityDto<LegalPerson> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new LegalPersonDomainEntityDto();
            long clientId = 0;
            if (parentEntityName == EntityName.Client && parentEntityId.HasValue)
            {
                clientId = parentEntityId.Value;
            }
            else if (!string.IsNullOrEmpty(extendedInfo) && long.TryParse(Regex.Match(extendedInfo, @"ClientId=(\d+)").Groups[1].Value, out clientId))
            {
            }

            dto.ClientRef = clientId > 0 
                ? 
                new EntityReference { Id = clientId, Name = _finder.Find<Client>(x => x.Id == clientId).Select(x => x.Name).Single() }
                : new EntityReference();

            return dto;
        }
    }
}