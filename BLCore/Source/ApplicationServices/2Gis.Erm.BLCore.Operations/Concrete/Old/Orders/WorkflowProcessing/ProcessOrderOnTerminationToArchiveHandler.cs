using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    // TODO : ручной перевод теперь запрещен, удалить как устаканится.
    public sealed class ProcessOrderOnTerminationToArchiveHandler : RequestHandler<ProcessOrderOnTerminationToArchiveRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ProcessOrderOnTerminationToArchiveHandler(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override EmptyResponse Handle(ProcessOrderOnTerminationToArchiveRequest request)
        {
            /* В документе "Сделка" изменить этап на «Сервис». Выгрузить / изменить к текущему, значение атрибута "Фактический доход" в документ "Сделка". 
             * Атрибут "Заказ.К оплате (факт)" в атрибут "Сделка.Фактический доход".
             */

            // Показатели сделки уже пересчитаны при переводе в статус "На расторжении".

            // Т.к. это ручной перевод в статус "Архив", то поле AmountToWithdraw может быть не равным нулю (если ещё не прошло списание).
            _subRequestProcessor.HandleSubRequest(new CalculateReleaseWithdrawalsRequest { Order = request.Order, UpdateAmountToWithdrawOnly = true }, Context);

            return Response.Empty;
        }
    }
}