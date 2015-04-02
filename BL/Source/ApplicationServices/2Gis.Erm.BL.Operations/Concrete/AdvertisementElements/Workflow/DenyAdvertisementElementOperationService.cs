using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;
using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow
{
    public sealed class DenyAdvertisementElementOperationService : IDenyAdvertisementElementOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IUpdateAggregateRepository<AdvertisementElementStatus> _updateAdvertisementElementStatusRepository;
        private readonly ICreateAggregateRepository<AdvertisementElementDenialReason> _createAdvertisementDenialReasonsAggregateRepository;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly IDenialReasonReadModel _denialReasonReadModel;
        private readonly IDeleteAggregateRepository<AdvertisementElementDenialReason> _deleteAdvertisementElementDenialReasonsAggregateRepository;

        private readonly INotifyAboutAdvertisementElementRejectionOperationService _notifyAboutAdvertisementElementRejectionOperationService;

        public DenyAdvertisementElementOperationService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory operationScopeFactory,
            IAdvertisementReadModel advertisementReadModel,
            IUpdateAggregateRepository<AdvertisementElementStatus> updateAdvertisementElementStatusRepository,
            ICreateAggregateRepository<AdvertisementElementDenialReason> createAdvertisementDenialReasonsAggregateRepository,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            INotifyAboutAdvertisementElementRejectionOperationService notifyAboutAdvertisementElementRejectionOperationService,
            IDenialReasonReadModel denialReasonReadModel,
            IDeleteAggregateRepository<AdvertisementElementDenialReason> deleteAdvertisementElementDenialReasonsAggregateRepository)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _notifyAboutAdvertisementElementRejectionOperationService = notifyAboutAdvertisementElementRejectionOperationService;
            _denialReasonReadModel = denialReasonReadModel;
            _deleteAdvertisementElementDenialReasonsAggregateRepository = deleteAdvertisementElementDenialReasonsAggregateRepository;
            _updateAdvertisementElementStatusRepository = updateAdvertisementElementStatusRepository;
            _operationScopeFactory = operationScopeFactory;
            _advertisementReadModel = advertisementReadModel;
            _createAdvertisementDenialReasonsAggregateRepository = createAdvertisementDenialReasonsAggregateRepository;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(DenyAdvertisementElementIdentity.Instance);
            }

            if (denialReasons == null || !denialReasons.Any())
            {
                throw new AdvertisementElementDenyingWithoutReasonsException(BLResources.YouHaveToSpecifyReasonToDenyAdvertisementElement);
            }
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            Deny(currentStatus, denialReasons);
        }

        public int Deny(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<DenyAdvertisementElementIdentity>())
            {
                var count = 0;

                currentStatus.Status = (int)AdvertisementElementStatusValue.Invalid;
                _updateAdvertisementElementStatusRepository.Update(currentStatus);
                operationScope.Updated(currentStatus);

                var inactiveDenialREasons = _denialReasonReadModel.GetInactiveDenialReasons(denialReasons.Select(x => x.DenialReasonId).ToArray());
                if (inactiveDenialREasons.Any())
                {
                    throw new AdvertisementElementDenyingWithInactiveReasonException(BLResources.YouCannotSpecifyInactiveDenialReason);
                }

                // У статуса нет самостоятельного Id. Он использует Id ЭРМ
                var advertisementElementId = currentStatus.Id;
                var currentElementDenialReasons = _advertisementReadModel.GetElementDenialReasonIds(advertisementElementId);

                foreach (var currentElementDenialReasonId in currentElementDenialReasons)
                {
                    _deleteAdvertisementElementDenialReasonsAggregateRepository.Delete(currentElementDenialReasonId);
                }

                foreach (var denialReason in denialReasons)
                {
                    count += _createAdvertisementDenialReasonsAggregateRepository.Create(denialReason);
                    operationScope.Added(denialReason);
                }

                var orderIds = _advertisementReadModel.GetDependedOrderIdsByAdvertisementElements(new[] { advertisementElementId });
                if (orderIds.Count > 0)
                {
                    _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                                {
                                                                                                    OrderId = x,
                                                                                                    ChangedAspects =
                                                                                                        new[]
                                                                                                            {
                                                                                                                OrderValidationRuleGroup.AdvertisementMaterialsValidation
                                                                                                            }
                                                                                                }));
                }

                _notifyAboutAdvertisementElementRejectionOperationService.Notify(advertisementElementId);

                operationScope.Complete();

                return count;
            }
        }
    }
}