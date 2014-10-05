using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class RegisterOrderStateChangesOperationService : IRegisterOrderStateChangesOperationService
    {
        private readonly IInvalidateCachedValidationResultAggregateService _invalidateCachedValidationResultAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public RegisterOrderStateChangesOperationService(
            IInvalidateCachedValidationResultAggregateService invalidateCachedValidationResultAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _invalidateCachedValidationResultAggregateService = invalidateCachedValidationResultAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Changed(long orderId, params OrderValidationRuleGroup[] aspects)
        {
            Changed(new[] { new OrderChangesDescriptor { OrderId = orderId, ChangedAspects = aspects } });
        }

        public void Changed(IEnumerable<OrderChangesDescriptor> changedOrderDescriptors)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<RegisterOrderStateChangesIdentity>())
            {
                // TODO {all, 24.09.2014}: рассмотреть возможность/необходимость реакции на уведомление об изменении заказа не только путем сброса кэша, но и путем, реального изменения заказа (например, через fake update) - чтобы инициировать изменение timestamp => появление новой версии + операции по заказу, на что смогут отреагировать выгружальщики в шину и т.п.
                // также возможно сигнатуру данного метода можно будет доработать чтобы он получал набор экземпляров изменившихся заказов заказов, выполнял нужные действия, и возвращал уже измененные экземпляры вызывающему коду, чтобы при выполнении каки-то доп. действий не возникало ложного optimistic concurrency из-за многократного независимого изменения заказа в одном и том же usecase
                var changedOrdersRegistry = new Dictionary<long, HashSet<OrderValidationRuleGroup>>();
                foreach (var changedOrder in changedOrderDescriptors)
                {
                    HashSet<OrderValidationRuleGroup> aspects;
                    if (!changedOrdersRegistry.TryGetValue(changedOrder.OrderId, out aspects))
                    {
                        aspects = new HashSet<OrderValidationRuleGroup>();
                        changedOrdersRegistry.Add(changedOrder.OrderId, aspects);
                    }

                    foreach (var orderAspect in changedOrder.ChangedAspects)
                    {
                        aspects.Add(orderAspect);
                    }
                }

                var processedOrdersDescriptors = 
                        changedOrdersRegistry
                            .Select(currentOrderEntry => new OrderChangesDescriptor
                                                            {
                                                                OrderId = currentOrderEntry.Key, 
                                                                ChangedAspects = currentOrderEntry.Value
                                                            });

                _invalidateCachedValidationResultAggregateService.Invalidate(processedOrdersDescriptors);
                
                scope.Complete();
            }
        }
    }
}
