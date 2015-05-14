using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

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
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public ChangeLegalPersonRequisitesHandler(
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            ILegalPersonRepository legalPersonRepository,
            ICheckInnService checkInnService,
            IOperationScopeFactory scopeFactory,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
            _checkInnService = checkInnService;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override EmptyResponse Handle(ChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ChangeRequisitesIdentity.Instance);
            }

            var entity = _legalPersonReadModel.GetLegalPerson(request.LegalPersonId);

            if (!entity.IsActive)
            {
                throw new ChangeInactiveLegalPersonRequisitesException(BLFlexResources.ChangingRequisitesOfInactiveLegalPersonIsForbidden);
            }

            entity.LegalName = request.LegalName;
            entity.ShortName = request.ShortName;
            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeRequisitesIdentity>())
            {
                if (entity.LegalPersonTypeEnum == LegalPersonType.NaturalPerson)
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
