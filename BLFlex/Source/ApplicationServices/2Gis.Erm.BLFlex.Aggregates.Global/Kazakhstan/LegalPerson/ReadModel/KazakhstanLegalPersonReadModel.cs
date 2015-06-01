using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Kazakhstan;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Kazakhstan.LegalPerson.ReadModel
{
    public class KazakhstanLegalPersonReadModel : IKazakhstanLegalPersonReadModel
    {
        private readonly IFinder _finder;

        public KazakhstanLegalPersonReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public CheckForDublicatesResultDto CheckIfExistsIdentityCardDuplicate(long legalPersonId, string identityCardNumber)
        {
            var ids = _finder.Find(BusinessEntitySpecs.BusinessEntity.Find.ByProperty(IdentityCardNumberIdentity.Instance.Id, identityCardNumber))
                             .Map(q => q.Select(x => x.EntityId.Value))
                             .Many();

            var legalPersons = _finder.Find(Specs.Find.ByIds<Platform.Model.Entities.Erm.LegalPerson>(ids) &&
                                            Specs.Find.ExceptById<Platform.Model.Entities.Erm.LegalPerson>(legalPersonId))
                                      .Map(q => q.Select(person => new { person.Id, person.IsActive, person.IsDeleted }))
                                      .Many();

            var result = new CheckForDublicatesResultDto
            {
                ActiveDublicateExists = legalPersons.Any(x => x.IsActive && !x.IsDeleted),
                InactiveDublicateExists = legalPersons.Any(x => !x.IsActive && !x.IsDeleted),
                DeletedDublicateExists = legalPersons.Any(x => x.IsDeleted)
            };

            return result;
        }
    }
}