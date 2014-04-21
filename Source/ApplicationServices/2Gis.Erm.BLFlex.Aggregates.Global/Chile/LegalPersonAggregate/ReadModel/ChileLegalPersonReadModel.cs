using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel
{
    // FIXME {all, 10.04.2014}: в процессе доработок по EAV - отвязать этот класс от базового LegalPersonReadModel (и от базовой асбтракции ILegalPersonReadModel), оставив в нем только BusinessModel специфичные операции
    public sealed class ChileLegalPersonReadModel : LegalPersonReadModel, IChileLegalPersonReadModel
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<ChileLegalPersonPart> _legalPersonPartConverter;
        private readonly IBusinessEntityPropertiesConverter<ChileLegalPersonProfilePart> _legalPersonProfilePartConverter;

        public ChileLegalPersonReadModel(IFinder finder,
            ISecureFinder secureFinder,
            IBusinessEntityPropertiesConverter<ChileLegalPersonPart> legalPersonPartConverter,
            IBusinessEntityPropertiesConverter<ChileLegalPersonProfilePart> legalPersonProfilePartConverter)
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
            return legalPerson.Parts.Cast<ChileLegalPersonPart>().Select(part => _finder.Single(part, _legalPersonPartConverter));
        }

        public override IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPersonProfile legalPersonProfile)
        {
            return legalPersonProfile.Parts.Cast<ChileLegalPersonProfilePart>().Select(part => _finder.Single(part, _legalPersonProfilePartConverter));
        }

        public EntityReference GetCommuneReference(long legalPersonId)
        {
            var numericCommuneId = _finder.Find(BusinessEntitySpecs.BusinessEntity.Find.ByReferencedEntity(legalPersonId))
                                   .Select(x => x.BusinessEntityPropertyInstances
                                                 .Where(y => y.PropertyId == CommuneIdIdentity.Instance.Id)
                                                 .Select(y => y.NumericValue)
                                                 .FirstOrDefault())
                                   .SingleOrDefault();

            if (numericCommuneId == null)
            {
                return null;
            }

            var communeId = Convert.ToInt64(numericCommuneId);
            var communeName = _finder.Find(Specs.Find.ById<DictionaryEntityInstance>(communeId))
                                     .SelectMany(x => x.DictionaryEntityPropertyInstances)
                                     .Where(x => x.PropertyId == NameIdentity.Instance.Id)
                                     .Select(x => x.TextValue)
                                     .SingleOrDefault();

            return new EntityReference(communeId, communeName);
        }
    }
}
