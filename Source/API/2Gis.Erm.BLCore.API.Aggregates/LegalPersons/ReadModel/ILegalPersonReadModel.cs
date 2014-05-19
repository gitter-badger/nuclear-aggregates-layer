using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using System.Collections.Generic;


using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel
{
    public interface ILegalPersonReadModel : IAggregateReadModel<LegalPerson>
    {
        LegalPerson GetLegalPerson(long legalPersonId);
        bool HasAnyLegalPersonProfiles(long legalPersonId);
        LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId);
        PaymentMethod? GetPaymentMethod(long legalPersonId);

        IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPerson x);
        IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPersonProfile legalPersonProfile);

        T GetLegalPersonDto<T>(long entityId)
            where T : LegalPersonDomainEntityDto, new();

        T GetLegalPersonProfileDto<T>(long entityId)
            where T : LegalPersonProfileDomainEntityDto, new();
    }
}