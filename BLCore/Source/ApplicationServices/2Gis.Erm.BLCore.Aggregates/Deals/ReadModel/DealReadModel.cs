using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel
{
    public sealed class DealReadModel : IDealReadModel
    {
        private readonly IFinder _finder;

        public DealReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Deal GetDeal(long id)
        {
            return _finder.Find(Specs.Find.ById<Deal>(id)).One();
        }

        public bool HasOrders(long dealId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForDeal(dealId) && Specs.Find.ActiveAndNotDeleted<Order>()).Any();
        }

        public IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds)
        {
            var result = _finder
                    .Find(Specs.Find.ByIds<Deal>(dealIds))
                    .Map(q => q.Select(deal => new DealActualizeDuringWithdrawalDto
                        {
                            Deal = deal,
                            HasInactiveLocks = deal.Orders.Any(o => o.Locks.Any(l => !l.IsActive && !l.IsDeleted))
                        }))
                    .Many();
            return result;
        }

        public IEnumerable<Deal> GetDealsByMainFirmIds(IEnumerable<long> mainFirmIds)
        {
            return _finder.Find(DealSpecs.Deals.Find.ByMainFirms(mainFirmIds)).Many();
        }

        public IEnumerable<Deal> GetDealsByClientId(long clientId)
        {
            var clientAndChild = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ClientChild(clientId)).Map(q => q.Select(s => (long?)s.ChildClientId))
                                        .Many()
                                        .Union(new[] { (long?)clientId });
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Deal>() && DealSpecs.Deals.Find.ByClientIds(clientAndChild)).Many();
        }

        public DealAndFirmNamesDto GetRelatedDealAndFirmNames(long dealId, long firmId)
        {
            return _finder.Find(DealSpecs.FirmDeals.Find.ByDealAndFirmIds(dealId, firmId) && Specs.Find.NotDeleted<FirmDeal>())
                          .Map(q => q.Select(x => new DealAndFirmNamesDto
                              {
                                  DealName = x.Deal.Name,
                                  FirmName = x.Firm.Name
                              }))
                          .Top();
        }

        public DealAndLegalPersonNamesDto GetRelatedDealAndLegalPersonNames(long dealId, long legalPersonId)
        {
            return _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDealAndLegalPersonIds(dealId, legalPersonId) && Specs.Find.NotDeleted<LegalPersonDeal>())
                          .Map(q => q.Select(x => new DealAndLegalPersonNamesDto
                              {
                                  DealName = x.Deal.Name,
                                  LegalPersonName = x.LegalPerson.LegalName
                              }))
                          .Top();
        }

        public bool AreThereAnyLegalPersonsForDeal(long dealId)
        {
            return _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDeal(dealId) && Specs.Find.NotDeleted<LegalPersonDeal>()).Any();
        }

        public LegalPersonDeal GetMainLegalPersonForDeal(long dealId)
        {
            return
                _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDeal(dealId) && Specs.Find.NotDeleted<LegalPersonDeal>() && DealSpecs.LegalPersonDeals.Find.Main())
                       .One();
        }

        public LegalPersonDeal GetLegalPersonDeal(long dealId, long legalPersonId)
        {
            return _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDealAndLegalPersonIds(dealId, legalPersonId) && Specs.Find.NotDeleted<LegalPersonDeal>()).One();
        }

        public LegalPersonDeal GetLegalPersonDeal(long entityId)
        {
            return _finder.Find(Specs.Find.ById<LegalPersonDeal>(entityId)).One();
        }

        public IEnumerable<string> GetDealLegalPersonNames(long dealId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Deal>(dealId))
                          .SelectMany(deal => deal.LegalPersonDeals)
                          .Where(Specs.Find.NotDeleted<LegalPersonDeal>())
                          .Select(link => link.LegalPerson.ShortName)
                          .ToArray();
        }

        public IEnumerable<string> GetDealFirmNames(long dealId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Deal>(dealId))
                          .SelectMany(deal => deal.FirmDeals)
                          .Where(Specs.Find.NotDeleted<FirmDeal>())
                          .Select(link => link.Firm.Name)
                          .ToArray();
        }

        public bool IsLinkTheLastOneForDeal(long id, long dealId)
        {
            var exitstAnotherLegalPersonForDeal = _finder.Find(Specs.Find.ExceptById<LegalPersonDeal>(id)
                                                               && DealSpecs.LegalPersonDeals.Find.ByDeal(dealId)
                                                               && Specs.Find.NotDeleted<LegalPersonDeal>())
                                                         .Any();
            return !exitstAnotherLegalPersonForDeal;
        }
    }
}