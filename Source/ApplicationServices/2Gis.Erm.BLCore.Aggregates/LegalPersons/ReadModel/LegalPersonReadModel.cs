using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public class LegalPersonReadModel : ILegalPersonReadModel
    {
        private readonly IFinder _finder;
        private readonly IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> _legalPersonProfilePartPropertiesConverter;

        public LegalPersonReadModel(
            IFinder finder,
            IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> legalPersonProfilePartPropertiesConverter)
        {
            _finder = finder;
            _legalPersonProfilePartPropertiesConverter = legalPersonProfilePartPropertiesConverter;
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId)).Single();
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId, BusinessModel businessModel)
        {
            var entityInstanceDto = GetBusinessEntityInstanceDto(legalPersonProfileId, businessModel).Single();
            var legalPersonProfilePart = _legalPersonProfilePartPropertiesConverter.ConvertFromDynamicEntityInstance(entityInstanceDto.EntityInstance,
                                                                                                                     entityInstanceDto.PropertyInstances);
            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId)).Single();
            legalPersonProfile.Parts = new List<LegalPersonProfilePart> { legalPersonProfilePart };

            return legalPersonProfile;
        }

        public BusinessEntityInstanceDto GetBusinessEntityInstanceDtoForLegalPersonProfilePart(LegalPersonProfilePart legalPersonProfilePart,
                                                                                               BusinessModel businessModel)
        {
            var dto = GetBusinessEntityInstanceDto(legalPersonProfilePart.Id, businessModel).SingleOrDefault();

            var propertyInstances = dto != null ? dto.PropertyInstances : new Collection<BusinessEntityPropertyInstance>();
            var tuple = _legalPersonProfilePartPropertiesConverter.ConvertToDynamicEntityInstance(legalPersonProfilePart, propertyInstances, null);

            return new BusinessEntityInstanceDto
                {
                    EntityInstance = tuple.Item1,
                    PropertyInstances = tuple.Item2
                };
        }

        private IQueryable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(long legalPersonId, BusinessModel businessModel)
        {
            var findSpec = BusinessEntitySpecs.BusinessEntity.Find.ByReferencedEntity(legalPersonId) &&
                           BusinessEntitySpecs.BusinessEntity.Find.ByBusinessModel(businessModel);

            return _finder.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(BusinessEntitySpecs.BusinessEntity.Select.BusinessEntityInstanceDto(),
                                                                                   findSpec);
        }
    }
}