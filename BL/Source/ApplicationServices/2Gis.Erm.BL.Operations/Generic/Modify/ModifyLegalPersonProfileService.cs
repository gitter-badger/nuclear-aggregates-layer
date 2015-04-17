using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Operations.Generic.Modify
{
    public sealed class ModifyLegalPersonProfileService : IModifyBusinessModelEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonProfileConsistencyRuleContainer _legalPersonProfileConsistencyRuleContainer;
        private readonly ICreateAggregateRepository<LegalPersonProfile> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPersonProfile> _updateRepository;
        private readonly IBusinessModelEntityObtainer<LegalPersonProfile> _entityObtainer;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public ModifyLegalPersonProfileService(
            ILegalPersonProfileConsistencyRuleContainer legalPersonProfileConsistencyRuleContainer,
            IBusinessModelEntityObtainer<LegalPersonProfile> entityObtainer,
            ICreateAggregateRepository<LegalPersonProfile> createRepository,
            IUpdateAggregateRepository<LegalPersonProfile> updateRepository,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _entityObtainer = entityObtainer;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _legalPersonReadModel = legalPersonReadModel;
            _legalPersonProfileConsistencyRuleContainer = legalPersonProfileConsistencyRuleContainer;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _entityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            var legalPersonWithProfiles = _legalPersonReadModel.GetLegalPersonWithProfileExistenceInfo(entity.LegalPersonId);

            if (legalPersonWithProfiles == null)
            {
                throw new EntityNotFoundException(typeof(LegalPerson), entity.LegalPersonId);
            }

            if (_legalPersonReadModel.IsThereLegalPersonProfileDuplicate(entity.Id, entity.LegalPersonId, entity.Name))
            {
                throw new EntityIsNotUniqueException(typeof(LegalPersonProfile), BLResources.LegalPersonProfileNameIsNotUnique);
            }

            var rules = _legalPersonProfileConsistencyRuleContainer.GetApplicableRules(legalPersonWithProfiles.LegalPerson, entity);

            foreach (var rule in rules)
            {
                rule.Apply(entity);
            }

            if (!legalPersonWithProfiles.LegalPersonHasProfiles)
            {
                entity.IsMainProfile = true;
            }

            if (entity.IsNew())
            {
                _createRepository.Create(entity);
            }
            else
            {
                _updateRepository.Update(entity);
            }

            return entity.Id;
        }
    }
}
