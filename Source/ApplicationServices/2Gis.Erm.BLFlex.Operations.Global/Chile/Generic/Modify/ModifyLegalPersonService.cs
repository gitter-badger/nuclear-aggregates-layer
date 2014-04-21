using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public class ModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IChileAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICreatePartableEntityAggregateService<LegalPerson, LegalPerson> _createService;
        private readonly IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson> _updateService;
        private readonly ICheckInnService _checkRutService;

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
            _checkRutService = checkRutService;
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

            if (string.IsNullOrEmpty(entity.Inn))
            {
                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Inn));
            }

            string rutError;
            if (_checkRutService.TryGetErrorMessage(entity.Inn, out rutError))
            {
                throw new NotificationException(rutError);
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
