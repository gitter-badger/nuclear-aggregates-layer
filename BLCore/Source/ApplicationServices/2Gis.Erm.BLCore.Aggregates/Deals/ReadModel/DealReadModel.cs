using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            return _finder.Find(Specs.Find.ById<Deal>(id)).SingleOrDefault();
        }

        public bool HasOrders(long dealId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForDeal(dealId) && Specs.Find.ActiveAndNotDeleted<Order>()).Any();
        }

        public IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds)
        {
            var result = _finder
                    .Find(Specs.Find.ByIds<Deal>(dealIds))
                    .Select(deal => new DealActualizeDuringWithdrawalDto
                        {
                            Deal = deal,
                            HasInactiveLocks = deal.Orders.Any(o => o.Locks.Any(l => !l.IsActive && !l.IsDeleted))
                        })
                    .ToArray();
            return result;
        }

        public IEnumerable<Deal> GetDealsByMainFirmIds(IEnumerable<long> mainFirmIds)
        {
            return _finder.Find(DealSpecs.Deals.Find.ByMainFirms(mainFirmIds)).ToArray();
        }

        public IEnumerable<Deal> GetDealsByClientId(long clientId)
        {
            var clientAndChild = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ClientChild(clientId)).Select(s => (long?)s.ChildClientId).ToArray().Union(new[] { (long?)clientId });
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Deal>() && DealSpecs.Deals.Find.ByClientIds(clientAndChild));
        }

        public DealAndFirmNamesDto GetRelatedDealAndFirmNames(long dealId, long firmId)
        {
            return _finder.Find(DealSpecs.FirmDeals.Find.ByDealAndFirmIds(dealId, firmId) && Specs.Find.NotDeleted<FirmDeal>())
                          .Select(x => new DealAndFirmNamesDto
                              {
                                  DealName = x.Deal.Name,
                                  FirmName = x.Firm.Name
                              })
                          .FirstOrDefault();
        }

        public DealAndLegalPersonNamesDto GetRelatedDealAndLegalPersonNames(long dealId, long legalPersonId)
        {
            return _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDealAndLegalPersonIds(dealId, legalPersonId) && Specs.Find.NotDeleted<LegalPersonDeal>())
                          .Select(x => new DealAndLegalPersonNamesDto
                              {
                                  DealName = x.Deal.Name,
                                  LegalPersonName = x.LegalPerson.LegalName
                              })
                          .FirstOrDefault();
        }

        public bool AreThereAnyLegalPersonsForDeal(long dealId)
        {
            return _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDeal(dealId) && Specs.Find.NotDeleted<LegalPersonDeal>()).Any();
        }

        public LegalPersonDeal GetMainLegalPersonForDeal(long dealId)
        {
            return
                _finder.Find(DealSpecs.LegalPersonDeals.Find.ByDeal(dealId) && Specs.Find.NotDeleted<LegalPersonDeal>() && DealSpecs.LegalPersonDeals.Find.Main())
                       .SingleOrDefault();
        }

        public LegalPersonDeal GetLegalPersonDeal(long dealId, long legalPersonId)
        {
            return _finder.FindOne(DealSpecs.LegalPersonDeals.Find.ByDealAndLegalPersonIds(dealId, legalPersonId) && Specs.Find.NotDeleted<LegalPersonDeal>());
        }

        public LegalPersonDeal GetLegalPersonDeal(long entityId)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPersonDeal>(entityId));
        }

        public IEnumerable<string> GetDealLegalPersonNames(long dealId)
        {
            return _finder.Find(Specs.Find.ById<Deal>(dealId))
                          .SelectMany(deal => deal.LegalPersonDeals)
                          .Where(Specs.Find.NotDeleted<LegalPersonDeal>())
                          .Select(link => link.LegalPerson.ShortName)
                          .ToArray();
        }

        public IEnumerable<string> GetDealFirmNames(long dealId)
        {
            return _finder.Find(Specs.Find.ById<Deal>(dealId))
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