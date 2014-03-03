using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonDtoService : GetDomainEntityDtoServiceBase<LegalPerson>, IChileAdapted
    {
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly ISecureFinder _finder;

        public ChileGetLegalPersonDtoService(IUserContext userContext, ILegalPersonReadModel legalPersonReadModel, ISecureFinder finder)
            : base(userContext)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _finder = finder;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
            var hasProfiles = _legalPersonReadModel.HasAnyLegalPersonProfiles(entityId);
            var clientReference = _legalPersonReadModel.GetClientReference(entityId);
            var communeReference = _legalPersonReadModel.GetCommuneReference(entityId);

            var entityPart = legalPerson.Parts.OfType<LegalPersonPart>().Single();

            var modelDto = new ChileLegalPersonDomainEntityDto
                                      {
                                          Id = legalPerson.Id,
                                          LegalName = legalPerson.LegalName,
                                          LegalPersonTypeEnum = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                                          LegalAddress = legalPerson.LegalAddress,
                                          Rut = legalPerson.Inn,
                                          OperationsKind = entityPart.OperationsKind,
                                          CommuneRef = communeReference,
                                          ClientRef = clientReference,
                                          Comment = legalPerson.Comment,
                                          OwnerRef = new EntityReference { Id = legalPerson.OwnerCode, Name = null },
                                          CreatedByRef = new EntityReference { Id = legalPerson.CreatedBy, Name = null },
                                          CreatedOn = legalPerson.CreatedOn,
                                          IsActive = legalPerson.IsActive,
                                          IsDeleted = legalPerson.IsDeleted,
                                          ModifiedByRef = new EntityReference { Id = legalPerson.ModifiedBy, Name = null },
                                          ModifiedOn = legalPerson.ModifiedOn,
                                          HasProfiles = hasProfiles,
                                          Timestamp = legalPerson.Timestamp
                                      };
            return modelDto;
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