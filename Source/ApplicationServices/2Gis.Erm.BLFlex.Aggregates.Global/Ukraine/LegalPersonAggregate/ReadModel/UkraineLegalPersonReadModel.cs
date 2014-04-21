using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.LegalPersonAggregate.ReadModel
{
    // FIXME {all, 10.04.2014}: в процессе доработок по EAV - отвязать этот класс от базового LegalPersonReadModel (и от базовой асбтракции ILegalPersonReadModel), оставив в нем только BusinessModel специфичные операции
    public sealed class UkraineLegalPersonReadModel : LegalPersonReadModel, IUkraineLegalPersonReadModel
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<UkraineLegalPersonPart> _legalPersonPartConverter;
        private readonly IBusinessEntityPropertiesConverter<UkraineLegalPersonProfilePart> _legalPersonProfilePartConverter;

        public UkraineLegalPersonReadModel(IFinder finder,
            ISecureFinder secureFinder,
            IBusinessEntityPropertiesConverter<UkraineLegalPersonPart> legalPersonPartConverter,
            IBusinessEntityPropertiesConverter<UkraineLegalPersonProfilePart> legalPersonProfilePartConverter)
            : base(finder, secureFinder)
        {
            _finder = finder;
            _legalPersonPartConverter = legalPersonPartConverter;
            _legalPersonProfilePartConverter = legalPersonProfilePartConverter;
        }

        public override LegalPerson GetLegalPerson(long legalPersonId)
        {
            return _finder.GetEntityWithPart(Specs.Find.ById<LegalPerson>(legalPersonId), _legalPersonPartConverter);
        }

        public override LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.GetEntityWithPart(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId), _legalPersonProfilePartConverter);
        }

        public override IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPerson legalPerson)
        {
            return legalPerson.Parts.Cast<UkraineLegalPersonPart>().Select(part => _finder.Single(part, _legalPersonPartConverter));
        }

        public override IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPersonProfile legalPersonProfile)
        {
            return legalPersonProfile.Parts.Cast<UkraineLegalPersonProfilePart>().Select(part => _finder.Single(part, _legalPersonProfilePartConverter));
        }

        public bool AreThereAnyActiveEgrpouDuplicates(long legalPersonId, string egrpou)
        {
            var duplicatesQuery = _finder.Find(BusinessEntitySpecs.BusinessEntity.Find.ByProperty(EgrpouIdentity.Instance.Id, egrpou))
                                         .Where(x => x.EntityId != legalPersonId).Select(x => x.EntityId);

            return _finder.FindAll<LegalPerson>().Join(duplicatesQuery, x => x.Id, y => y.Value, (x, y) => x).Any(x => x.IsActive && !x.IsDeleted);
        }
    }
}
