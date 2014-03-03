using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public interface ILegalPersonReadModel : IAggregateReadModel<LegalPerson>
    {
        LegalPerson GetLegalPerson(long legalPersonId);
        EntityReference GetClientReference(long legalPersonId);
        EntityReference GetCommuneReference(long legalPersonId);
        bool HasAnyLegalPersonProfiles(long legalPersonId);
        LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId);
        BusinessEntityInstanceDto GetBusinessEntityInstanceDto(LegalPersonPart legalPersonPart);
        BusinessEntityInstanceDto GetBusinessEntityInstanceDto(LegalPersonProfilePart legalPersonProfilePart);
    }
}