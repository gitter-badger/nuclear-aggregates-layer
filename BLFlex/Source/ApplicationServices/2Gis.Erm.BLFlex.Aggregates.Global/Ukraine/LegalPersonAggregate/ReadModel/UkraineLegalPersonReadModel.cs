using System.Linq;

using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.LegalPersonAggregate.ReadModel
{
    public sealed class UkraineLegalPersonReadModel : IUkraineLegalPersonReadModel
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public UkraineLegalPersonReadModel(IQuery query, IFinder finder)
        {
            _query = query;
            _finder = finder;
        }

        public bool AreThereAnyActiveEgrpouDuplicates(long legalPersonId, string egrpou)
        {
            var duplicatesQuery = _query.For(BusinessEntitySpecs.BusinessEntity.Find.ByProperty(EgrpouIdentity.Instance.Id, egrpou))
                                        .Where(x => x.EntityId != legalPersonId)
                                        .Select(x => x.EntityId);

            return _query.For<LegalPerson>().Join(duplicatesQuery, x => x.Id, y => y.Value, (x, y) => x).Any(x => x.IsActive && !x.IsDeleted);
        }
    }
}
