using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations
{
    public class RequestStateApplicationService : IRequestStateApplicationService
    {
        private readonly IGetDomainEntityDtoService<OrderProcessingRequest> _domainEntityDtoService;
        private readonly ICommonLog _logger;

        public RequestStateApplicationService(IGetDomainEntityDtoService<OrderProcessingRequest> domainEntityDtoService, ICommonLog logger)
        {
            _domainEntityDtoService = domainEntityDtoService;
            _logger = logger;
        }

        public IOrderRequestStateDescription[] GetState(IEnumerable<long> requestIds)
        {
            try
            {
                var states = requestIds
                    .Select(x => (OrderProcessingRequestDomainEntityDto)_domainEntityDtoService.GetDomainEntityDto(x, true, null, EntityName.None, null))
                    .Select(x =>
                        {
                            IOrderRequestStateDescription currentStateDescription;
                            switch (x.State)
                            {
                                case OrderProcessingRequestState.Undefined:
                                    throw new InvalidOperationException("Заявка на создание заказа имеет неопределенный статус");
                                case OrderProcessingRequestState.Opened:
                                    currentStateDescription = new OpenedOrderRequestStateDescription();
                                    break;
                                case OrderProcessingRequestState.Cancelled:
                                    currentStateDescription = new CancelledOrderRequestStateDescription();
                                    break;
                                case OrderProcessingRequestState.Completed:
                                    currentStateDescription = new CompletedOrderRequestStateDescription
                                        {
                                            OrderId = x.RenewedOrderRef.Id.Value
                                        };
                                    break;
                                case OrderProcessingRequestState.Pending:
                                    currentStateDescription = new PendingOrderRequestStateDescription();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            currentStateDescription.RequestId = x.Id;
                            return currentStateDescription;
                        })
                    .ToArray();
                
                return states;
            }
            catch (BusinessLogicException ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.FatalFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<OrderProcessingErrorDescription>(new OrderProcessingErrorDescription(BLResources.InTheOrderProcessingRequestsServiceErrorOccured));
            }
        }
    }
}
