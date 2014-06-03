﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.Old
{
    public sealed class CyprusEditLegalPersonHandler : RequestHandler<EditRequest<LegalPerson>, EmptyResponse>, ICyprusAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateLegalPersonRepository;
        private readonly ICreateAggregateRepository<LegalPerson> _createLegalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CyprusEditLegalPersonHandler(
            ISubRequestProcessor subRequestProcessor,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            IUpdateAggregateRepository<LegalPerson> updateLegalPersonRepository,
            ICreateAggregateRepository<LegalPerson> createLegalPersonRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _updateLegalPersonRepository = updateLegalPersonRepository;
            _createLegalPersonRepository = createLegalPersonRepository;
        }

        protected override EmptyResponse Handle(EditRequest<LegalPerson> request)
        {
            if (!request.Entity.IsNew())
            {
                var personWithProfiles = _legalPersonRepository.GetLegalPersonWithProfiles(request.Entity.Id);
                if (!personWithProfiles.Profiles.Any())
                {
                    throw new NotificationException(BLResources.MustMakeLegalPersonProfile);
                }
            }

            var modelLegalPersonType = (LegalPersonType)request.Entity.LegalPersonTypeEnum;
            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    if (string.IsNullOrEmpty(request.Entity.Inn))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Inn));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }

                    break;
                case LegalPersonType.Businessman:
                    if (string.IsNullOrEmpty(request.Entity.Inn))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Inn));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }

                    break;
                case LegalPersonType.NaturalPerson:

                    if (string.IsNullOrEmpty(request.Entity.PassportNumber))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PassportNumber));
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }

            if (request.Entity.IsNew())
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = request.Entity }, Context);
            }

            try
            {
                using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(request.Entity))
                {
                    var isNewLegalPerson = request.Entity.IsNew();

                    if (isNewLegalPerson)
                    {
                        _createLegalPersonRepository.Create(request.Entity);
                        operationScope.Added<LegalPerson>(request.Entity.Id);
                    }
                    else
                    {
                        _updateLegalPersonRepository.Update(request.Entity);
                        operationScope.Updated<LegalPerson>(request.Entity.Id);
                    }

                    operationScope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new NotificationException(BLResources.ErrorWhileSavingLegalPerson, ex);
            }

            return Response.Empty;
        }
    }
}
