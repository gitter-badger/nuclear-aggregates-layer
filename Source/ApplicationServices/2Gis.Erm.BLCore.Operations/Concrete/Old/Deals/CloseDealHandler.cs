using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Crm.SdkTypeProxy;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals
{
    public sealed class CloseDealHandler : RequestHandler<CloseDealRequest, EmptyResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealRepository _dealRepository;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserContext _userContext;

        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public CloseDealHandler(
            IDealReadModel dealReadModel,
            IDealRepository dealRepository,
            IMsCrmSettings msCrmSettings, 
            ISecurityServiceEntityAccess securityServiceEntityAccess, 
            IUserContext userContext)
        {
            _dealReadModel = dealReadModel;
            _dealRepository = dealRepository;
            _msCrmSettings = msCrmSettings;
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

            // error if deal have open crm actions
            if (_msCrmSettings.EnableReplication)
            {
                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();

                    var crmDeal =
                        crmDataContext.GetEntities(EntityName.opportunity).SingleOrDefault(
                            x => x.GetPropertyValue<Guid>("opportunityid") == deal.ReplicationCode);
                    if (crmDeal != null)
                    {
                        var crmActivities = crmDeal.GetRelatedEntities("Opportunity_ActivityPointers");
                        if (crmActivities.Select(
                            activity => Convert.ToInt32(activity["statuscode"].Value, CultureInfo.InvariantCulture))
                            .Any(status => status == 1))
                        {
                            throw new NotificationException(BLResources.NeedToCloseAllActivities);
                        }
                    }
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            if (_dealRepository.CheckIfDealHasOpenOrders(request.Id))
            {
                throw new NotificationException(BLResources.CloseDealNeedToCloseAllOrders);
            }

            // validate security
            if (
                !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update, Platform.Model.Entities.EntityName.Deal,
                                                              _userContext.Identity.Code, deal.Id, deal.OwnerCode, null))
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
