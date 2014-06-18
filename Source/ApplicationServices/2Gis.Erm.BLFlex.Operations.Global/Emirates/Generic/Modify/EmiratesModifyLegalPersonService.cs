﻿using System;

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
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public class EmiratesModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IEmiratesAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICheckInnService _checkCommercialLicenseService;
        private readonly ICreateAggregateRepository<LegalPerson> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;

        public EmiratesModifyLegalPersonService(
            IPublicService publicService,
            ILegalPersonReadModel readModel,
            IBusinessModelEntityObtainer<LegalPerson> obtainer,
            ICheckInnService checkCommercialLicenseService,
            ICreateAggregateRepository<LegalPerson> createRepository,
            IUpdateAggregateRepository<LegalPerson> updateRepository)
        {
            _readModel = readModel;
            _publicService = publicService;
            _obtainer = obtainer;
            _checkCommercialLicenseService = checkCommercialLicenseService;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
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

            string commercialLicenseError;
            if (_checkCommercialLicenseService.TryGetErrorMessage(entity.Inn, out commercialLicenseError))
            {
                throw new NotificationException(commercialLicenseError);
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
