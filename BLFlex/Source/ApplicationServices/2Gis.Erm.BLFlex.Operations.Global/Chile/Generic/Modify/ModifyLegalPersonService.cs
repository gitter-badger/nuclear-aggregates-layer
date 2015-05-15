using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public class ModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IChileAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICreateAggregateRepository<LegalPerson> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkRutService;

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
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Inn));
            }

            string rutError;
            if (_checkRutService.TryGetErrorMessage(entity.Inn, out rutError))
            {
                throw new NotificationException(rutError);
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
