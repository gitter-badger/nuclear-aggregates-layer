using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public interface IDealReadModel : IAggregateReadModel<Deal>
    {
        Deal GetDeal(long id);
        bool HasOrders(long dealId);
        IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds);
        IEnumerable<Deal> GetDealsByMainFirmIds(IEnumerable<long> mainFirmIds);

        IEnumerable<Deal> GetDealsByClientId(long clientId);
        DealAndFirmNamesDto GetRelatedDealAndFirmNames(long dealId, long firmId);
        DealAndLegalPersonNamesDto GetRelatedDealAndLegalPersonNames(long dealId, long legalPersonId);
        bool AreThereAnyLegalPersonsForDeal(long dealId);
        LegalPersonDeal GetMainLegalPersonForDeal(long dealId);
        LegalPersonDeal GetLegalPersonDeal(long dealId, long legalPersonId);
        LegalPersonDeal GetLegalPersonDeal(long entityId);
        IEnumerable<string> GetDealLegalPersonNames(long dealId);
        IEnumerable<string> GetDealFirmNames(long dealId);
        bool IsLinkTheLastOneForDeal(long id, long dealId);
    }
}