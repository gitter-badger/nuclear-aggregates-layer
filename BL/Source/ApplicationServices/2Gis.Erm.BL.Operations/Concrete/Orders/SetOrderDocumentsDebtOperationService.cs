using DoubleGis.Erm.BL.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.Operations.Concrete.Orders
{
    public sealed class SetOrderDocumentsDebtOperationService : ISetOrderDocumentsDebtOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISetOrderDocumentsDebtAggregateService _specifyOrderDocumentsDebtAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IReleaseReadModel _releaseReadModel;

        public SetOrderDocumentsDebtOperationService(IOrderReadModel orderReadModel,
                                                     ISetOrderDocumentsDebtAggregateService specifyOrderDocumentsDebtAggregateService,
                                                     IOperationScopeFactory scopeFactory,
                                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                                     IUserContext userContext,
                                                     IReleaseReadModel releaseReadModel)
        {
            _orderReadModel = orderReadModel;
            _specifyOrderDocumentsDebtAggregateService = specifyOrderDocumentsDebtAggregateService;
            _scopeFactory = scopeFactory;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _releaseReadModel = releaseReadModel;
        }

        public void Set(long orderId, DocumentsDebt documentsDebt, string documentsDebtComment)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), orderId);
            }

            if (!_functionalAccessService.HasOrderChangeDocumentsDebtAccess(order.SourceOrganizationUnitId, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(SetOrderDocumentsDebtIdentity.Instance);
            }

            if (_releaseReadModel.HasFinalReleaseInProgress(order.DestOrganizationUnitId,
                                                            new TimePeriod(order.BeginDistributionDate, order.EndDistributionDateFact)))
            {
                throw new SetOrderDocumentsDebtOperationIsBlockedByReleaseException(BLResources.OperationCannotBePerformedSinceReleaseIsInProgress);
            }

            using (var scope = _scopeFactory.CreateNonCoupled<SetOrderDocumentsDebtIdentity>())
            {
                _specifyOrderDocumentsDebtAggregateService.Set(order, documentsDebt, documentsDebtComment);
                scope.Complete();
            }
        }
    }
}