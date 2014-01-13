using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnApprovedToOnRegistrationHandler : RequestHandler<ProcessOrderOnApprovedToOnRegistrationRequest, EmptyResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDealRepository _dealRepository;

        public ProcessOrderOnApprovedToOnRegistrationHandler(IAccountRepository accountRepository, IDealRepository dealRepository)
        {
            _accountRepository = accountRepository;
            _dealRepository = dealRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnApprovedToOnRegistrationRequest request)
        {
            var order = request.Order;
            
            // Проверить был ли выпуск по заказу (есть ли блокировка), если не было  - продолжаем.
            var lockExists = _accountRepository.IsNonDeletedLocksExists(order.Id);
            if (lockExists)
            {
                throw new NotificationException(BLResources.ProcessOrder_LockExists);
            }

            /* Если есть сделка и по этой сделке больше нет заказов в статусе "Одобрено", "На расторжении", "Архиве",
             * изменить этап в сделке с «Заказ одобрен в выпуск» на "Сформирован Заказ".
             */
            if (order.DealId.HasValue)
            {
                _dealRepository.SetOrderFormedStage(order.DealId.Value, order.Id);
            }

            // NOTE: Необходимо передавать через воркфлу заказа не сам объект заказ, а контекст операции
            // И только на этапе сохранения обновлять заказ из контекста
            order.ApprovalDate = null;

            return Response.Empty;
        }
    }
}