using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel
{
    public interface ILegalPersonReadModel : IAggregateReadModel<LegalPerson>
    {
        string GetLegalPersonName(long legalPersonId);
        bool HasAnyLegalPersonProfiles(long legalPersonId);
        PaymentMethod? GetPaymentMethod(long legalPersonId);
        string GetActiveLegalPersonNameWithSpecifiedInn(string inn);
        LegalPersonType GetLegalPersonType(long legalPersonId);

        LegalPerson GetLegalPerson(long legalPersonId);
        LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId);
        IEnumerable<LegalPersonProfile> GetProfilesByLegalPerson(long legalPersonId);
        IEnumerable<long> GetLegalPersonProfileIds(long legalPersonId);
        long? GetMainLegalPersonProfileId(long legalPersonId);
        int? GetLegalPersonOrganizationDgppid(long legalPersonId);
        bool DoesLegalPersonHaveActiveNotArchivedAndNotRejectedOrders(long legalPersonId);
    }
}