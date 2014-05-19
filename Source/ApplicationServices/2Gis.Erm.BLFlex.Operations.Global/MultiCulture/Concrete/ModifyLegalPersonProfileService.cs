using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete
{
    public sealed class ModifyLegalPersonProfileService : IModifyBusinessModelEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonProfileConsistencyRuleContainer _legalPersonProfileConsistencyRuleContainer;
        private readonly ICreatePartableEntityAggregateService<LegalPerson, LegalPersonProfile> _createService;
        private readonly IUpdatePartableEntityAggregateService<LegalPerson, LegalPersonProfile> _updateService;
        private readonly IBusinessModelEntityObtainer<LegalPersonProfile> _entityObtainer;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public ModifyLegalPersonProfileService(
            ILegalPersonProfileConsistencyRuleContainer legalPersonProfileConsistencyRuleContainer,
            ICreatePartableEntityAggregateService<LegalPerson, LegalPersonProfile> createService, 
            IUpdatePartableEntityAggregateService<LegalPerson, LegalPersonProfile> updateService,
            IBusinessModelEntityObtainer<LegalPersonProfile> entityObtainer,
            ILegalPersonRepository legalPersonRepository,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _entityObtainer = entityObtainer;
            _legalPersonRepository = legalPersonRepository;
            _legalPersonReadModel = legalPersonReadModel;
            _createService = createService;
            _updateService = updateService;
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
                var partDtos = _legalPersonReadModel.GetBusinessEntityInstanceDto(entity).ToArray();

                if (entity.IsNew())
                {
                    _createService.Create(entity, partDtos);
                }
                else
                {
                    _updateService.Update(entity, partDtos);
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
