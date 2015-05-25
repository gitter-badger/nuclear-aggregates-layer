using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.LegalPersons
{
    public sealed class CyprusChangeLegalPersonRequisitesHandler : RequestHandler<CyprusChangeLegalPersonRequisitesRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IUpdateAggregateRepository<LegalPerson> _legalPersonUpdateRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public CyprusChangeLegalPersonRequisitesHandler(
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IUpdateAggregateRepository<LegalPerson> legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonUpdateRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override EmptyResponse Handle(CyprusChangeLegalPersonRequisitesRequest request)
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

            var legalPersonType = entity.LegalPersonTypeEnum;

            // три стратегии замены реквизитов для трех разных типов юрлиц
            if (legalPersonType == LegalPersonType.NaturalPerson)
            {
                entity.LegalName = request.LegalName;
                entity.CardNumber = request.CardNumber;
                entity.PassportNumber = request.PassportNumber;
            }
            else if (legalPersonType == LegalPersonType.Businessman)
            {
                entity.LegalName = request.LegalName;
                entity.CardNumber = request.CardNumber;
                entity.PassportNumber = request.PassportNumber;
                entity.LegalAddress = request.LegalAddress;
                entity.Inn = request.Inn; // на самом деле, TIC
                entity.VAT = request.VAT;
            }
            else if (legalPersonType == LegalPersonType.LegalPerson)
            {
                entity.LegalName = request.LegalName;
                entity.LegalAddress = request.LegalAddress;
                entity.Inn = request.Inn; // на самом деле, TIC
                entity.VAT = request.VAT;
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeRequisitesIdentity>())
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity }, Context);
                _legalPersonUpdateRepository.Update(entity);

                operationScope
                    .Updated<LegalPerson>(entity.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}
