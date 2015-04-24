using System.Collections.Generic;
using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow
{
    public class TransferAdvertisementElementToReadyForValidationOpeationService : ITransferAdvertisementElementToReadyForValidationOpeationService
    {
        private readonly IUpdateAggregateRepository<AdvertisementElementStatus> _updateAdvertisementElementStatusRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public TransferAdvertisementElementToReadyForValidationOpeationService(
            IUpdateAggregateRepository<AdvertisementElementStatus> updateAdvertisementElementStatusRepository,
            IOperationScopeFactory operationScopeFactory,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _updateAdvertisementElementStatusRepository = updateAdvertisementElementStatusRepository;
            _operationScopeFactory = operationScopeFactory;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            // Действительно, при наличии ФП операцию выполнить нельзя 
            if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(TransferAdvertisementElementToReadyForValidationIdentity.Instance);
            }
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            TransferToReadyForValidation(currentStatus);
        }

        public int TransferToReadyForValidation(AdvertisementElementStatus currentStatus)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<TransferAdvertisementElementToReadyForValidationIdentity>())
            {
                currentStatus.Status = (int)AdvertisementElementStatusValue.ReadyForValidation;
                var count = _updateAdvertisementElementStatusRepository.Update(currentStatus);
                operationScope.Updated(currentStatus);

                operationScope.Complete();
                return count;
            }
        }
    }
}