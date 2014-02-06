using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public interface ILegalPersonReadModel : IAggregateReadModel<LegalPerson>
    {
        LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId);
        LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId, BusinessModel businessModel);
        BusinessEntityInstanceDto GetBusinessEntityInstanceDtoForLegalPersonProfilePart(LegalPersonProfilePart legalPersonProfilePart,
                                                                                        BusinessModel businessModel);
    }
}