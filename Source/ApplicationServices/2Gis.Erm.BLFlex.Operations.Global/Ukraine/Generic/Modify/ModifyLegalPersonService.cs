using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public class ModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IUkraineAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICreatePartableEntityAggregateService<LegalPerson, LegalPerson> _createService;
        private readonly IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson> _updateService;
        private readonly ICheckInnService _checkIpnService;

        public ModifyLegalPersonService(
            IPublicService publicService,
            ILegalPersonReadModel readModel,
            IBusinessModelEntityObtainer<LegalPerson> obtainer,
            ICreatePartableEntityAggregateService<LegalPerson, LegalPerson> createService,
            IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson> updateService,
            ICheckInnService checkRutService)
        {
            _readModel = readModel;
            _publicService = publicService;
            _obtainer = obtainer;
            _createService = createService;
            _updateService = updateService;
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

            var part = entity.UkrainePart();
            if (part.TaxationType == TaxationType.WithVat && string.IsNullOrEmpty(entity.Inn))
            {
                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Inn));
            }

            if (!string.IsNullOrEmpty(entity.Inn))
            {
                const int LegalPersonIpnLength = 12;
                if ((LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson && entity.Inn.Length != LegalPersonIpnLength)
                {
                    throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidIpn, LegalPersonIpnLength));
                }

                const int BusinessmanIpnLength = 10;
                if ((LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.Businessman && entity.Inn.Length != BusinessmanIpnLength)
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
            if ((LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson && part.Egrpou.Length != LegalPersonEgrpouLength)
            {
                throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, LegalPersonEgrpouLength));
            }

            const int BusinessmanEgrpouLength = 10;
            if ((LegalPersonType)entity.LegalPersonTypeEnum == LegalPersonType.Businessman && part.Egrpou.Length != BusinessmanEgrpouLength)
            {
                throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, BusinessmanEgrpouLength));
            }

            if (string.IsNullOrEmpty(entity.LegalAddress))
            {
                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.LegalAddress));
            }

            if (entity.IsNew())
            {
                _publicService.Handle(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity });
            }

            try
            {
                var dtos = _readModel.GetBusinessEntityInstanceDto(entity).ToArray();

                if (entity.IsNew())
                {
                    _createService.Create(entity, dtos);
                }
                else
                {
                    _updateService.Update(entity, dtos);
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
