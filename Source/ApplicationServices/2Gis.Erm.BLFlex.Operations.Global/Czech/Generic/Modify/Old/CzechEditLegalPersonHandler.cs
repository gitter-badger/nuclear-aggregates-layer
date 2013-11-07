using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.Old
{
    public sealed class CzechEditLegalPersonHandler : RequestHandler<EditRequest<LegalPerson>, EmptyResponse>, ICzechAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CzechEditLegalPersonHandler(
            ISubRequestProcessor subRequestProcessor,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory)
        {
            _subRequestProcessor = subRequestProcessor;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<LegalPerson> request)
        {
            if (request.Entity.Id == 0)
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = request.Entity }, Context);
            }

            if (request.Entity.Id != 0)
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

            try
            {
                using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(request.Entity))
                {
                    var isNewLegalPerson = request.Entity.IsNew();

                    _legalPersonRepository.CreateOrUpdate(request.Entity);

                    if (isNewLegalPerson)
                    {
                        operationScope.Added<LegalPerson>(request.Entity.Id);
                    }
                    else
                    {
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