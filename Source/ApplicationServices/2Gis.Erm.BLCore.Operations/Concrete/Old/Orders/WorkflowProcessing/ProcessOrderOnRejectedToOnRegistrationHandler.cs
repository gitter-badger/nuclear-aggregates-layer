using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnRejectedToOnRegistrationHandler : RequestHandler<ProcessOrderOnRejectedToOnRegistrationRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ProcessOrderOnRejectedToOnRegistrationHandler(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override EmptyResponse Handle(ProcessOrderOnRejectedToOnRegistrationRequest request)
        {
            var order = request.Order;
            if (order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }

            /* У заказа есть связный документ "Сделка". Выгрузить / изменить к текущему, значение атрибута "Предполагаемый доход" в документ "Сделка". 
             * Атрибут "Заказ.К оплате (план)" в атрибут "Сделка.Предполагаемый доход". 
             * Нужно увеличить значение атрибута "Предполагаемый доход" в документ "Сделка" на размер значения атрибута "Заказ.К оплате (план)".
             */
            if (order.DealId.HasValue)
            {
                _subRequestProcessor.HandleSubRequest(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { order.DealId.Value } }, Context);
            }

            return Response.Empty;
        }
    }
}
