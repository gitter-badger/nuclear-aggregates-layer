using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals
{
    public interface IDealRepository : IAggregateRootRepository<Deal>,
                                       IAssignAggregateRepository<Deal>,
                                       IChangeAggregateClientRepository<Deal>
    {
        void Update(Deal deal);
        void Add(Deal deal);
        int Assign(Deal deal, long ownerCode);
        bool CheckIfDealHasOpenOrders(long dealId);
        void CloseDeal(Deal deal, CloseDealReason closeReason, string closeReasonOther, string comment);
        void ReopenDeal(Deal deal);
        ClientAndFirmForDealInfo GetClientAndFirmForDealInfo(Deal deal);
        int SetOrderApprovedForReleaseStage(long dealId);
        int SetOrderFormedStage(long dealId, long orderId);
        int DecreaseDealEstimatedProfit(Deal deal, decimal estimatedProfitDelta);
        DealLegalPersonDto GetDealLegalPerson(long dealId);
    }
}