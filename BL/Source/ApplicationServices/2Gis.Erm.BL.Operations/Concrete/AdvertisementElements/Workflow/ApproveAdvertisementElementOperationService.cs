using System.Collections.Generic;
using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
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
    public sealed class ApproveAdvertisementElementOperationService : IApproveAdvertisementElementOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IUpdateAggregateRepository<AdvertisementElementStatus> _updateAdvertisementElementStatusRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IDeleteAggregateRepository<AdvertisementElementDenialReason> _deleteAdvertisementElementDenialReasonsAggregateRepository;
        private readonly IAdvertisementReadModel _advertisementReadModel;

        public ApproveAdvertisementElementOperationService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IUpdateAggregateRepository<AdvertisementElementStatus> updateAdvertisementElementStatusRepository,
            IOperationScopeFactory operationScopeFactory,
            IDeleteAggregateRepository<AdvertisementElementDenialReason> deleteAdvertisementElementDenialReasonsAggregateRepository,
            IAdvertisementReadModel advertisementReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _updateAdvertisementElementStatusRepository = updateAdvertisementElementStatusRepository;
            _operationScopeFactory = operationScopeFactory;
            _deleteAdvertisementElementDenialReasonsAggregateRepository = deleteAdvertisementElementDenialReasonsAggregateRepository;
            _advertisementReadModel = advertisementReadModel;
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ApproveAdvertisementElementIdentity.Instance);
            }
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            Approve(currentStatus, denialReasons);
        }

        public int Approve(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ApproveAdvertisementElementIdentity>())
            {
                var count = 0;

                currentStatus.Status = (int)AdvertisementElementStatusValue.Valid;
                count += _updateAdvertisementElementStatusRepository.Update(currentStatus);
                operationScope.Updated(currentStatus);

                var advertisementElementDenialReasonIds = _advertisementReadModel.GetElementDenialReasonIds(currentStatus.Id);

                foreach (var denialReasonId in advertisementElementDenialReasonIds)
                {
                    count += _deleteAdvertisementElementDenialReasonsAggregateRepository.Delete(denialReasonId);
                    operationScope.Deleted<AdvertisementElementDenialReason>(denialReasonId);
                }

                operationScope.Complete();

                return count;
            }
        }
    }
}