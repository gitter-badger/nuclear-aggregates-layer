using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
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
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.Old
{
    public sealed class EditLegalPersonHandler : RequestHandler<EditRequest<LegalPerson>, EmptyResponse>, IRussiaAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateLegalPersonRepository;
        private readonly ICreateAggregateRepository<LegalPerson> _createLegalPersonRepository;
        private readonly ICheckInnService _checkInnService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public EditLegalPersonHandler(
            ISubRequestProcessor subRequestProcessor,
            ICheckInnService checkInnService,
            IOperationScopeFactory scopeFactory,
            IUpdateAggregateRepository<LegalPerson> updateLegalPersonRepository,
            ICreateAggregateRepository<LegalPerson> createLegalPersonRepository,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _checkInnService = checkInnService;
            _scopeFactory = scopeFactory;
            _updateLegalPersonRepository = updateLegalPersonRepository;
            _createLegalPersonRepository = createLegalPersonRepository;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override EmptyResponse Handle(EditRequest<LegalPerson> request)
        {
            var modelLegalPersonType = request.Entity.LegalPersonTypeEnum;
            if (modelLegalPersonType == LegalPersonType.LegalPerson || modelLegalPersonType == LegalPersonType.Businessman)
            {
                string innErrorMessage;
                if (_checkInnService.TryGetErrorMessage(request.Entity.Inn, out innErrorMessage))
                {
                    throw new NotificationException(innErrorMessage);
                }
            }

            if (request.Entity.IsNew())
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = request.Entity }, Context);
            }

            if (!request.Entity.IsNew())
            {
                var personWithProfiles = _legalPersonReadModel.GetLegalPersonWithProfileExistenceInfo(request.Entity.Id);
                if (!personWithProfiles.LegalPersonHasProfiles)
                {
                    throw new NotificationException(BLResources.MustMakeLegalPersonProfile);
                }
            }

            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    if (string.IsNullOrEmpty(request.Entity.Inn))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Inn));
                    }

                    if (string.IsNullOrEmpty(request.Entity.Kpp))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Kpp));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }

                    break;
                case LegalPersonType.Businessman:
                    if (string.IsNullOrEmpty(request.Entity.Inn))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Inn));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }

                    break;
                case LegalPersonType.NaturalPerson:
                    if (string.IsNullOrEmpty(request.Entity.PassportSeries))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.PassportSeries));
                    }

                    if (string.IsNullOrEmpty(request.Entity.PassportNumber))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.PassportNumber));
                    }

                    if (string.IsNullOrEmpty(request.Entity.PassportIssuedBy))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.PassportIssuedBy));
                    }

                    if (string.IsNullOrEmpty(request.Entity.RegistrationAddress))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.RegistrationAddress));
                    }

                    break;
                default:
                    throw new NotSupportedException();
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
