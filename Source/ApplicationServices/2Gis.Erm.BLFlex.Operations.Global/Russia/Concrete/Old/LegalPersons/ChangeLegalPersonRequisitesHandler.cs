using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.LegalPersons
{
    public sealed class ChangeLegalPersonRequisitesHandler : RequestHandler<ChangeLegalPersonRequisitesRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ICheckInnService _checkInnService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFinder _finder;

        public ChangeLegalPersonRequisitesHandler(
            ISubRequestProcessor subRequestProcessor, 
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            ILegalPersonRepository legalPersonRepository,
            ICheckInnService checkInnService,
            IOperationScopeFactory scopeFactory,
            IFinder finder)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
            _checkInnService = checkInnService;
            _scopeFactory = scopeFactory;
            _finder = finder;
        }

        protected override EmptyResponse Handle(ChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            var entity = _finder.FindOne(Specs.Find.ById<LegalPerson>(request.LegalPersonId));
            entity.LegalName = request.LegalName;
            entity.ShortName = request.ShortName;
            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeRequisitesIdentity>())
            {
                if (entity.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson)
                {
                    _legalPersonRepository.ChangeNaturalRequisites(entity, request.PassportSeries, request.PassportNumber, request.RegistrationAddress);
                }
                else
                {
                    string innErrorMessage;
                    if (_checkInnService.TryGetErrorMessage(request.Inn, out innErrorMessage))
                    {
                        throw new NotificationException(innErrorMessage);
                    }

                    _legalPersonRepository.ChangeLegalRequisites(entity, request.Inn, request.Kpp, request.LegalAddress);
                }

                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity }, Context);

                operationScope
                    .Updated<LegalPerson>(entity.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}
