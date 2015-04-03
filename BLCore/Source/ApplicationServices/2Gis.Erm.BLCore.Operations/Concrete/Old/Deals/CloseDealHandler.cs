using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals
{
    public sealed class CloseDealHandler : RequestHandler<CloseDealRequest, EmptyResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealRepository _dealRepository;
        private readonly IActivityReadService _activityReadService;
        private readonly IUserContext _userContext;

        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public CloseDealHandler(
            IDealReadModel dealReadModel,
            IDealRepository dealRepository,
            IActivityReadService activityReadService, 
            ISecurityServiceEntityAccess securityServiceEntityAccess, 
            IUserContext userContext)
        {
            _dealReadModel = dealReadModel;
            _dealRepository = dealRepository;
            _activityReadService = activityReadService;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
        }

        protected override EmptyResponse Handle(CloseDealRequest request)
        {
            if (request.CloseReason == CloseDealReason.None)
            {
                throw new NotificationException(BLResources.MustPickCloseReason);
            }

            var deal = _dealReadModel.GetDeal(request.Id);
            if (deal == null)
            {
                throw new NotificationException(string.Format(BLResources.CouldNotFindDeal, request.Id));
            }

            if (!deal.IsActive)
            {
                throw new NotificationException(BLResources.DealMustBeActive);
            }

            // error if deal have open activities
            if (_activityReadService.CheckIfOpenActivityExistsRegarding(EntityType.Instance.Deal(), deal.Id))
            {
                throw new NotificationException(BLResources.NeedToCloseAllActivities);
            }

            if (_dealRepository.CheckIfDealHasOpenOrders(request.Id))
            {
                throw new NotificationException(BLResources.CloseDealNeedToCloseAllOrders);
            }

            // validate security
            if (
                !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityType.Instance.Deal(),
                                                              _userContext.Identity.Code,
                                                              deal.Id,
                                                              deal.OwnerCode,
                                                              null))
            {
                throw new NotificationException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _dealRepository.CloseDeal(deal, request.CloseReason, request.CloseReasonOther, request.Comment);
   
                transaction.Complete();
            }

            return Response.Empty;
        }
    }
}
