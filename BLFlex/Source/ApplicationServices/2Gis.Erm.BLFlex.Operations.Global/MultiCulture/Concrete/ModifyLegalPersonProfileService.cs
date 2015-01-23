using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete
{
    public sealed class ModifyLegalPersonProfileService : IModifyBusinessModelEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonProfileConsistencyRuleContainer _legalPersonProfileConsistencyRuleContainer;
        private readonly ICreateAggregateRepository<LegalPersonProfile> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPersonProfile> _updateRepository;
        private readonly IBusinessModelEntityObtainer<LegalPersonProfile> _entityObtainer;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public ModifyLegalPersonProfileService(
            ILegalPersonProfileConsistencyRuleContainer legalPersonProfileConsistencyRuleContainer,
            IBusinessModelEntityObtainer<LegalPersonProfile> entityObtainer,
            ILegalPersonRepository legalPersonRepository,
            ICreateAggregateRepository<LegalPersonProfile> createRepository,
            IUpdateAggregateRepository<LegalPersonProfile> updateRepository)
        {
            _entityObtainer = entityObtainer;
            _legalPersonRepository = legalPersonRepository;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _legalPersonProfileConsistencyRuleContainer = legalPersonProfileConsistencyRuleContainer;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _entityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            var legalPersonWithProfiles = _legalPersonRepository.GetLegalPersonWithProfiles(entity.LegalPersonId);

            if (legalPersonWithProfiles == null)
            {
                throw new NotificationException(BLResources.LegalPersonNotFound);
            }

            if (legalPersonWithProfiles.Profiles.Any(legalPersonProfile => legalPersonProfile.Name == entity.Name && legalPersonProfile.Id != entity.Id))
            {
                throw new NotificationException(BLResources.LegalPersonProfileNameIsNotUnique);
            }

            var rules = _legalPersonProfileConsistencyRuleContainer.GetApplicableRules(legalPersonWithProfiles.LegalPerson, entity);

            foreach (var rule in rules)
            {
                rule.Apply(entity);
            }

            if (!legalPersonWithProfiles.Profiles.Any())
            {
                entity.IsMainProfile = true;
            }

            try
            {
                if (entity.IsNew())
                {
                    _createRepository.Create(entity);
                }
                else
                {
                    _updateRepository.Update(entity);
                }
            }
            catch (Exception ex)
            {
                throw new NotificationException(BLResources.ErrorWhileSavingLegalPersonProfile, ex);
            }

            return entity.Id;
        }
    }
}
