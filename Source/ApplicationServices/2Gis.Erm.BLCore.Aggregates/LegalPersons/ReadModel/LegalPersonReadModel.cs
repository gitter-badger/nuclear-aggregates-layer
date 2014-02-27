using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public class LegalPersonReadModel : ILegalPersonReadModel
    {
        private readonly IFinder _finder;
        private readonly IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance> _legalPersonPartPropertiesConverter;
        private readonly IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> _legalPersonProfilePartPropertiesConverter;

        public LegalPersonReadModel(
            IFinder finder,
            IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance> legalPersonPartPropertiesConverter,
            IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> legalPersonProfilePartPropertiesConverter)
        {
            _finder = finder;
            _legalPersonPartPropertiesConverter = legalPersonPartPropertiesConverter;
            _legalPersonProfilePartPropertiesConverter = legalPersonProfilePartPropertiesConverter;
        }

        public LegalPerson GetLegalPerson(long legalPersonId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).Single();
                var legalPersonProfilePart = _finder.SingleOrDefault(legalPersonId, _legalPersonPartPropertiesConverter.ConvertFromDynamicEntityInstance);
                legalPerson.Parts = new[] { legalPersonProfilePart };

                transactionScope.Complete();

                return legalPerson;
            }
        }

        public EntityReference GetClientReference(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => new EntityReference { Id = x.ClientId, Name = x.Client.Name })
                          .SingleOrDefault();
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

        public bool HasAnyLegalPersonProfiles(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).Any(x => x.LegalPersonProfiles.Any());
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId)).Single();
                var legalPersonProfilePart = _finder.SingleOrDefault(legalPersonProfileId,
                                                                     _legalPersonProfilePartPropertiesConverter.ConvertFromDynamicEntityInstance);
                legalPersonProfile.Parts = new[] { legalPersonProfilePart };

                transactionScope.Complete();

                return legalPersonProfile;
            }
        }

        public BusinessEntityInstanceDto GetBusinessEntityInstanceDto(LegalPersonPart legalPersonPart)
        {
            return _finder.Single(legalPersonPart, _legalPersonPartPropertiesConverter.ConvertToDynamicEntityInstance);
        }

        public BusinessEntityInstanceDto GetBusinessEntityInstanceDto(LegalPersonProfilePart legalPersonProfilePart)
        {
            return _finder.Single(legalPersonProfilePart, _legalPersonProfilePartPropertiesConverter.ConvertToDynamicEntityInstance);
        }
    }
}