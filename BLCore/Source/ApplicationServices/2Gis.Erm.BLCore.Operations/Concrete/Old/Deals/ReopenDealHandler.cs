using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals
{
    public sealed class ReopenDealHandler : RequestHandler<ReopenDealRequest, ReopenDealResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;
        private readonly IDealRepository _dealRepository;

        public ReopenDealHandler(
            IDealReadModel dealReadModel,
            IDealRepository dealRepository,
            ISecurityServiceEntityAccess securityServiceEntityAccess, 
            IUserContext userContext)
        {
            _dealReadModel = dealReadModel;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
            _dealRepository = dealRepository;
        }

        protected override ReopenDealResponse Handle(ReopenDealRequest request)
        {
            var response = new ReopenDealResponse();

            var deal = _dealReadModel.GetDeal(request.DealId);
            if (deal == null)
            {
                response.Message = BLResources.CouldNotFindDeal;
                return response;
            }

            if (deal.IsActive)
            {
                response.Message = string.Format(BLResources.DealMustBeNonActive);
                return response;
            }

            // validate security
            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityName.Deal,
                                                              _userContext.Identity.Code,
                                                              deal.Id,
                                                              deal.OwnerCode,
                                                              null))
            {
                throw new NotificationException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _dealRepository.ReopenDeal(deal);
                response.Message = BLResources.OK;
   
                transaction.Complete();
            }

            return response;
        }
    }
}
