using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.LegalPersons
{
    public sealed class ChileChangeLegalPersonRequisitesHandler : RequestHandler<ChileChangeLegalPersonRequisitesRequest, EmptyResponse>, IChileAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson> _updatePartsService;
        private readonly ICheckInnService _checkRutService;

        public ChileChangeLegalPersonRequisitesHandler(
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory, 
            ILegalPersonReadModel legalPersonReadModel, 
            IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson> updatePartsService, 
            ICheckInnService checkRutService)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _updatePartsService = updatePartsService;
            _checkRutService = checkRutService;
        }

        protected override EmptyResponse Handle(ChileChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            string rutFormatError;
            if (_checkRutService.TryGetErrorMessage(request.Rut, out rutFormatError))
            {
                throw new NotificationException(rutFormatError);
            }

            var entity = _legalPersonReadModel.GetLegalPerson(request.LegalPersonId);
            var legalPersonType = (LegalPersonType)entity.LegalPersonTypeEnum;

            // TODO {all, 26.02.2014}: ��������, ����� ������ ������, ���� � ��� ������ ���-�� ����� �� � ��. ����
            if (legalPersonType == LegalPersonType.Businessman || legalPersonType == LegalPersonType.LegalPerson)
            {
                entity.LegalName = request.LegalName;
                entity.LegalAddress = request.LegalAddress;
                entity.Inn = request.Rut;

                var chileLegalPersonPart = entity.ChilePart();
                chileLegalPersonPart.CommuneId = request.CommuneId;
                chileLegalPersonPart.OperationsKind = request.OperationsKind;
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeRequisitesIdentity>())
            {
                _subRequestProcessor.HandleSubRequest(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity }, Context);
                _legalPersonRepository.CreateOrUpdate(entity);

                var chileLegalPersonParts = _legalPersonReadModel.GetBusinessEntityInstanceDto(entity).ToArray();
                _updatePartsService.Update(entity, chileLegalPersonParts);

                operationScope
                    .Updated<LegalPerson>(entity.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}