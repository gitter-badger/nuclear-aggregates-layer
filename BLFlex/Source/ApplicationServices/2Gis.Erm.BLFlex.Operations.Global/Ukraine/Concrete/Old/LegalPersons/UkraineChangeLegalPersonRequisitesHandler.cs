using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.LegalPersons
{
    public sealed class UkraineChangeLegalPersonRequisitesHandler : RequestHandler<UkraineChangeLegalPersonRequisitesRequest, EmptyResponse>, IUkraineAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkIpnService;

        public UkraineChangeLegalPersonRequisitesHandler(ISubRequestProcessor subRequestProcessor,
                                                         ISecurityServiceFunctionalAccess functionalAccessService,
                                                         IUserContext userContext,
                                                         IOperationScopeFactory scopeFactory,
                                                         ILegalPersonReadModel legalPersonReadModel,
                                                         IUpdateAggregateRepository<LegalPerson> updateRepository,
                                                         ICheckInnService checkIpnService)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _updateRepository = updateRepository;
            _checkIpnService = checkIpnService;
        }

        protected override EmptyResponse Handle(UkraineChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ChangeRequisitesIdentity.Instance);
            }

            string ipnFormatError;
            if (_checkIpnService.TryGetErrorMessage(request.Ipn, out ipnFormatError))
            {
                throw new NotificationException(ipnFormatError);
            }

            var entity = _legalPersonReadModel.GetLegalPerson(request.LegalPersonId);
            if (!entity.IsActive)
            {
                throw new ChangeInactiveLegalPersonRequisitesException(BLFlexResources.ChangingRequisitesOfInactiveLegalPersonIsForbidden);
            }

            var legalPersonType = entity.LegalPersonTypeEnum;

            // TODO {all, 26.02.2014}: Возможно, стоит кидать ошибку, если к нам пришло что-то кроме ИП и Юр. лица
            if (legalPersonType == LegalPersonType.Businessman || legalPersonType == LegalPersonType.LegalPerson)
            {
                if (request.TaxationType == TaxationType.WithVat && string.IsNullOrEmpty(request.Ipn))
                {
                    throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Inn));
                }

                if (!string.IsNullOrEmpty(request.Ipn))
                {
                    const int LegalPersonIpnLength = 12;
                    if (legalPersonType == LegalPersonType.LegalPerson && request.Ipn.Length != LegalPersonIpnLength)
                    {
                        throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidIpn, LegalPersonIpnLength));
                    }

                    const int BusinessmanIpnLength = 10;
                    if (legalPersonType == LegalPersonType.Businessman && request.Ipn.Length != BusinessmanIpnLength)
                    {
                        throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidIpn, BusinessmanIpnLength));
                    }

                    string ipnError;
                    if (_checkIpnService.TryGetErrorMessage(request.Ipn, out ipnError))
                    {
                        throw new NotificationException(ipnError);
                    }
                }

                const int LegalPersonEgrpouLength = 8;
                if (legalPersonType == LegalPersonType.LegalPerson && request.Egrpou.Length != LegalPersonEgrpouLength)
                {
                    throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, LegalPersonEgrpouLength));
                }

                const int BusinessmanEgrpouLength = 10;
                if (legalPersonType == LegalPersonType.Businessman && request.Egrpou.Length != BusinessmanEgrpouLength)
                {
                    throw new NotificationException(string.Format(Resources.Server.Properties.BLResources.UkraineInvalidEgrpou, BusinessmanEgrpouLength));
                }

                if (string.IsNullOrEmpty(request.LegalAddress))
                {
                    throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
                }

                entity.LegalName = request.LegalName;
                entity.LegalAddress = request.LegalAddress;
                entity.Inn = request.Ipn;
                entity.Within<UkraineLegalPersonPart>().SetPropertyValue(x => x.TaxationType, request.TaxationType);
                entity.Within<UkraineLegalPersonPart>().SetPropertyValue(x => x.Egrpou, request.Egrpou);
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeRequisitesIdentity>())
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity }, Context);
                _updateRepository.Update(entity);

                operationScope
                    .Updated<LegalPerson>(entity.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}