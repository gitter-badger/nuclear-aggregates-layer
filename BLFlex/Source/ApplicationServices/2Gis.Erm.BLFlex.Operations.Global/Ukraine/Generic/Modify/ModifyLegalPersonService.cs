using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public class ModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IUkraineAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICreateAggregateRepository<LegalPerson> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkIpnService;

        public ModifyLegalPersonService(
            IPublicService publicService,
            ILegalPersonReadModel readModel,
            IBusinessModelEntityObtainer<LegalPerson> obtainer,
            ICreateAggregateRepository<LegalPerson> createRepository,
            IUpdateAggregateRepository<LegalPerson> updateRepository,
            ICheckInnService checkRutService)
        {
            _readModel = readModel;
            _publicService = publicService;
            _obtainer = obtainer;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _checkIpnService = checkRutService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (!entity.IsNew())
            {
                var hasProfiles = _readModel.HasAnyLegalPersonProfiles(entity.Id);
                if (!hasProfiles)
                {
                    throw new NotificationException(BLResources.MustMakeLegalPersonProfile);
                }
            }

            var taxationType = entity.Within<UkraineLegalPersonPart>().GetPropertyValue(x => x.TaxationType);
            if (taxationType == TaxationType.WithVat && string.IsNullOrEmpty(entity.Inn))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Inn));
            }

            if (!string.IsNullOrEmpty(entity.Inn))
            {
                const int LegalPersonIpnLength = 12;
                if (entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson && entity.Inn.Length != LegalPersonIpnLength)
                {
                    throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidIpn, LegalPersonIpnLength));
                }

                const int BusinessmanIpnLength = 10;
                if (entity.LegalPersonTypeEnum == LegalPersonType.Businessman && entity.Inn.Length != BusinessmanIpnLength)
                {
                    throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidIpn, BusinessmanIpnLength));
                }

                string ipnError;
                if (_checkIpnService.TryGetErrorMessage(entity.Inn, out ipnError))
                {
                    throw new NotificationException(ipnError);
                } 
            }

            const int LegalPersonEgrpouLength = 8;
            var egrpou = entity.Within<UkraineLegalPersonPart>().GetPropertyValue(x => x.Egrpou);
            if (entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson && egrpou.Length != LegalPersonEgrpouLength)
            {
                throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, LegalPersonEgrpouLength));
            }

            const int BusinessmanEgrpouLength = 10;
            if (entity.LegalPersonTypeEnum == LegalPersonType.Businessman && egrpou.Length != BusinessmanEgrpouLength)
            {
                throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, BusinessmanEgrpouLength));
            }

            if (string.IsNullOrEmpty(entity.LegalAddress))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
            }

            if (entity.IsNew())
            {
                _publicService.Handle(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity });
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
                throw new NotificationException(BLResources.ErrorWhileSavingLegalPerson, ex);
            }

            return entity.Id;
        }
    }
}
