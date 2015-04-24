using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow
{
    public class ResetAdvertisementElementToDraftOperationService : IResetAdvertisementElementToDraftOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IUpdateAggregateRepository<AdvertisementElementStatus> _updateAdvertisementElementStatusRepository;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public ResetAdvertisementElementToDraftOperationService(
            IOperationScopeFactory operationScopeFactory,
            IAdvertisementReadModel advertisementReadModel,
            IUpdateAggregateRepository<AdvertisementElementStatus> updateAdvertisementElementStatusRepository,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _updateAdvertisementElementStatusRepository = updateAdvertisementElementStatusRepository;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _operationScopeFactory = operationScopeFactory;
            _advertisementReadModel = advertisementReadModel;
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            // Действительно, при наличии ФП операцию выполнить нельзя
            if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ResetAdvertisementElementToDraftIdentity.Instance);
            }
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            ResetToDraft(currentStatus);
        }

        public int ResetToDraft(AdvertisementElementStatus currentStatus)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ResetAdvertisementElementToDraftIdentity>())
            {
                currentStatus.Status = (int)AdvertisementElementStatusValue.Draft;
                var count = _updateAdvertisementElementStatusRepository.Update(currentStatus);
                operationScope.Updated(currentStatus);

                var orderIds = _advertisementReadModel.GetDependedOrderIdsByAdvertisementElements(new[] { currentStatus.Id });
                if (orderIds.Count > 0)
                {
                    _registerOrderStateChangesOperationService.Changed(orderIds.Select(x =>
                                                                                       new OrderChangesDescriptor
                                                                                           {
                                                                                               OrderId = x,
                                                                                               ChangedAspects =
                                                                                                   new[]
                                                                                                       {
                                                                                                           OrderValidationRuleGroup.AdvertisementMaterialsValidation
                                                                                                       }
                                                                                           }));
                }

                operationScope.Complete();
                return count;
            }
        }
    }
}