using System.Linq;

using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.DTO;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.LegalPersons
{
    public class RussiaLegalPersonReadModel : IRussiaLegalPersonReadModel
    {
        private readonly IFinder _finder;

        public RussiaLegalPersonReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonForMergeDto GetInfoForMerge(long legalPersonId)
        {
            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPerson>()).One();
            if (legalPerson == null)
            {
                return null;
            }

            var relatedEntities = _finder.FindObsolete(Specs.Find.ById<LegalPerson>(legalPersonId))
                                         .Select(x => new
                                             {
                                                 x.Accounts,
                                                 AccountDetails = x.Accounts.SelectMany(y => y.AccountDetails.Where(z => z.IsActive && !z.IsDeleted)),
                                                 x.Orders,
                                                 x.Bargains,
                                                 ProfileIds = x.LegalPersonProfiles.Select(profile => profile.Id)
                                             })
                                         .Single();

            return new LegalPersonForMergeDto
                {
                    LegalPerson = legalPerson,
                    Accounts = relatedEntities.Accounts,
                    AccountDetails = relatedEntities.AccountDetails,
                    Orders = relatedEntities.Orders,
                    Bargains = relatedEntities.Bargains,
                    Profiles = _finder.Find(Specs.Find.ByIds<LegalPersonProfile>(relatedEntities.ProfileIds)).Many()
                };
        }
    }
}