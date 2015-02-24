using System;

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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.Old
{
    public sealed class CzechEditLegalPersonHandler : RequestHandler<EditRequest<LegalPerson>, EmptyResponse>, ICzechAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateLegalPersonRepository;
        private readonly ICreateAggregateRepository<LegalPerson> _createLegalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public CzechEditLegalPersonHandler(
            ISubRequestProcessor subRequestProcessor,
            IOperationScopeFactory scopeFactory,
            IUpdateAggregateRepository<LegalPerson> updateLegalPersonRepository,
            ICreateAggregateRepository<LegalPerson> createLegalPersonRepository, 
            ILegalPersonReadModel legalPersonReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _scopeFactory = scopeFactory;
            _updateLegalPersonRepository = updateLegalPersonRepository;
            _createLegalPersonRepository = createLegalPersonRepository;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override EmptyResponse Handle(EditRequest<LegalPerson> request)
        {
            if (!request.Entity.IsNew())
            {
                var personWithProfiles = _legalPersonReadModel.GetLegalPersonWithProfileExistanceInfo(request.Entity.Id);
                if (!personWithProfiles.LegalPersonHasProfiles)
                {
                    throw new NotificationException(BLResources.MustMakeLegalPersonProfile);
                }
            }
            
            var modelLegalPersonType = request.Entity.LegalPersonTypeEnum;
            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    if (string.IsNullOrEmpty(request.Entity.Ic))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Ic));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }
                    
                    if (string.IsNullOrEmpty(request.Entity.LegalName))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalName));
                    }


                    break;
                case LegalPersonType.Businessman:
                    if (string.IsNullOrEmpty(request.Entity.Ic))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Ic));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalAddress))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
                    }

                    if (string.IsNullOrEmpty(request.Entity.LegalName))
                    {
                        throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalName));
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