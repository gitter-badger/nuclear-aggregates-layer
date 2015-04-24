using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Kazakhstan.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.LegalPersons
{
    public class KazakhstanChangeLegalPersonRequisitesHandler : RequestHandler<KazakhstanChangeLegalPersonRequisitesRequest, EmptyResponse>, IKazakhstanAdapted
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkBinService;

        public KazakhstanChangeLegalPersonRequisitesHandler(ISubRequestProcessor subRequestProcessor,
                                                            ISecurityServiceFunctionalAccess functionalAccessService,
                                                            IUserContext userContext,
                                                            IOperationScopeFactory scopeFactory,
                                                            ILegalPersonReadModel legalPersonReadModel,
                                                            IUpdateAggregateRepository<LegalPerson> updateRepository,
                                                            ICheckInnService checkBinService)
        {
            _subRequestProcessor = subRequestProcessor;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _updateRepository = updateRepository;
            _checkBinService = checkBinService;
        }

        protected override EmptyResponse Handle(KazakhstanChangeLegalPersonRequisitesRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ChangeRequisitesIdentity.Instance);
            }

            string message;
            if (_checkBinService.TryGetErrorMessage(request.Bin, out message))
            {
                throw new NotificationException(string.Format(message, ResolveInnName(request.LegalPersonType)));
            }

            var entity = _legalPersonReadModel.GetLegalPerson(request.LegalPersonId);

            if (!entity.IsActive)
            {
                throw new ChangeInactiveLegalPersonRequisitesException(BLFlexResources.ChangingRequisitesOfInactiveLegalPersonIsForbidden);
            }


            if ((entity.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson ||
                 entity.LegalPersonTypeEnum == LegalPersonType.Businessman) &&
                string.IsNullOrEmpty(entity.LegalAddress))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
            }

            if (entity.LegalPersonTypeEnum == LegalPersonType.NaturalPerson)
            {
                ValidateIdentityCardNumber(entity.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardNumber));
            }

            entity.Inn = request.Bin;
            entity.LegalAddress = request.LegalAddress;
            entity.LegalName = request.LegalName;
            entity.LegalPersonTypeEnum = request.LegalPersonType;
            entity.Within<KazakhstanLegalPersonPart>().SetPropertyValue(x => x.IdentityCardNumber, request.IdentityCardNumber);
            entity.Within<KazakhstanLegalPersonPart>().SetPropertyValue(x => x.IdentityCardIssuedBy, request.IdentityCardIssuedBy);
            entity.Within<KazakhstanLegalPersonPart>().SetPropertyValue(x => x.IdentityCardIssuedOn, request.IdentityCardIssuedOn);

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

        private static void ValidateIdentityCardNumber(string identityCardNumber)
        {
            if (string.IsNullOrEmpty(identityCardNumber))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.IdentityCardNumber));
            }

            if (identityCardNumber.Length != 9)
            {
                throw new NotificationException(string.Format(BLResources.StringLengthLocalizedAttribute_ValidationErrorEqualsLimitations, MetadataResources.IdentityCardNumber, 9));
            }
        }

        private static string ResolveInnName(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return MetadataResources.Bin;
                case LegalPersonType.Businessman:
                    return MetadataResources.BinIin;
                case LegalPersonType.NaturalPerson:
                    return MetadataResources.Iin;
                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}