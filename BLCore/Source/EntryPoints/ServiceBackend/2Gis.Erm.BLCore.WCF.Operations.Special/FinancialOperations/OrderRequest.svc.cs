using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations
{
    public class OrderProcessingRequestsApplicationService : IOrderProcessingRequestsApplicationService
    {
        private readonly ITracer _tracer;
        private readonly ICreateOrderProlongationRequestOperationService _orderProlongationRequestService;
        private readonly ICreateOrderCreationRequestOperationService _orderCreationRequestService;

        public OrderProcessingRequestsApplicationService(
            ITracer tracer,
            ICreateOrderProlongationRequestOperationService orderProcessingRequestService,
            IUserContext userContext,
            ICreateOrderCreationRequestOperationService orderCreationRequestService,
            IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _orderProlongationRequestService = orderProcessingRequestService;
            _orderCreationRequestService = orderCreationRequestService;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public long ProlongateOrder(long orderId, short releaseCountPlan)
        {
            try
            {
                return _orderProlongationRequestService.CreateOrderProlongationRequest(orderId, releaseCountPlan, null);
            }
            catch (BusinessLogicException ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(ex.Message));
            }
            catch (Exception ex)
            {
                _tracer.FatalFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(BLResources.InTheOrderProcessingRequestsServiceErrorOccured));
            }
        }

        public long ProlongateOrderWithComment(long orderId, short releaseCountPlan, string description)
        {
            try
            {
                return _orderProlongationRequestService.CreateOrderProlongationRequest(orderId, releaseCountPlan, description);
            }
            catch (BusinessLogicException ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(ex.Message));
            }
            catch (Exception ex)
            {
                _tracer.FatalFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(BLResources.InTheOrderProcessingRequestsServiceErrorOccured));
            }
        }

        public long CreateOrder(long sourceProjectCode,
                                DateTime beginDistributionDate,
                                short releaseCountPlan,
                                long firmId,
                                long legalPersonProfileId,
                                string description)
        {
            try
            {
                return _orderCreationRequestService.CreateOrderRequest(sourceProjectCode,
                                                                       beginDistributionDate,
                                                                       releaseCountPlan,
                                                                       firmId,
                                                                       legalPersonProfileId,
                                                                       description);
            }
            catch (BusinessLogicException ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(ex.Message));
            }
            catch (Exception ex)
            {
                _tracer.FatalFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(BLResources.InTheOrderProcessingRequestsServiceErrorOccured));
            }
        }
    }
}
