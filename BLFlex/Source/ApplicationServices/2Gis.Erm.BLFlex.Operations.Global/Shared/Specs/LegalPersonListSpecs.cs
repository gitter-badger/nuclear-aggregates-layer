using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Specs
{
    public static class LegalPersonListSpecs
    {
        public static class Filter
        {
            [Obsolete("Убрать этот хак определения юр лиц после полноценной реализации рекламных кампаний")]
            public static Expression<Func<LegalPerson, bool>> ByDeal(long dealId, IFinder finder)
            {
                var dealLegalPersons = finder.Find(DealSpecs.LegalPersonDeals.Find.ByDeal(dealId) &&
                                                   Platform.DAL.Specifications.Specs.Find.NotDeleted<LegalPersonDeal>())
                                             .Select(x => x.LegalPersonId)
                                             .ToArray();

                if (dealLegalPersons.Any())
                {
                    return x => dealLegalPersons.Contains(x.Id);
                }

                return null;
            }

            public static Expression<Func<LegalPerson, bool>> ByOwner(bool forMe, long userCode)
            {
                if (forMe)
                {
                    return x => x.OwnerCode == userCode;
                }

                return x => x.OwnerCode != userCode;
            }

            public static Expression<Func<LegalPerson, bool>> WithDebt(decimal minDebtAmount)
            {
                return x => x.Accounts.Any(y => !y.IsDeleted && y.IsActive && y.Balance < minDebtAmount);
            }

            public static Expression<Func<LegalPerson, bool>> ByMyBranch(long userCode)
            {
                return x => x.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userCode);
            }

            public static Expression<Func<LegalPerson, bool>> WithUserOrders(long userCode)
            {
                return x => x.Orders.Any(y => !y.IsDeleted && y.IsActive && y.OwnerCode == userCode);
            }
        }
    }
}