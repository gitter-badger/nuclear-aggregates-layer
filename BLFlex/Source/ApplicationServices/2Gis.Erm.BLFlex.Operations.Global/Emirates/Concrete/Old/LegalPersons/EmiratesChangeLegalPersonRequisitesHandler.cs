using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.LegalPersons
{
    public sealed class EmiratesChangeLegalPersonRequisitesHandler :
        RequestHandler<EmiratesChangeLegalPersonRequisitesRequest, EmptyResponse>, IEmiratesAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkIpnService;

        public EmiratesChangeLegalPersonRequisitesHandler(
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ILegalPersonReadModel legalPersonReadModel,
            ICheckInnService checkIpnService,
            IUpdateAggregateRepository<LegalPerson> updateRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _checkIpnService = checkIpnService;
            _updateRepository = updateRepository;
        }

        protected override EmptyResponse Handle(EmiratesChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ChangeRequisitesIdentity.Instance);
            }

            string ipnFormatError;
            if (_checkIpnService.TryGetErrorMessage(request.CommercialLicense, out ipnFormatError))
            {
                throw new NotificationException(ipnFormatError);
            }

            var entity = _legalPersonReadModel.GetLegalPerson(request.LegalPersonId);

            if (!entity.IsActive)
            {
                throw new ChangeInactiveLegalPersonRequisitesException(BLFlexResources.ChangingRequisitesOfInactiveLegalPersonIsForbidden);
            }

            if (string.IsNullOrEmpty(request.LegalAddress))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
            }

            entity.LegalName = request.LegalName;
            entity.LegalAddress = request.LegalAddress;
            entity.Inn = request.CommercialLicense;

            entity.Within<EmiratesLegalPersonPart>().SetPropertyValue(x => x.CommercialLicenseBeginDate, request.CommercialLicenseBeginDate);
            entity.Within<EmiratesLegalPersonPart>().SetPropertyValue(x => x.CommercialLicenseEndDate, request.CommercialLicenseEndDate);

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