using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyOrderUsingHandlerService : IModifyBusinessModelEntityService<Order>
    {
        private readonly IFinder _finder;
        private readonly IBusinessModelEntityObtainer<Order> _businessModelEntityObtainer;
        private readonly IOrderRepository _orderRepository;
        private readonly IPublicService _publicService;

        public ModifyOrderUsingHandlerService(IFinder finder,
                                              IBusinessModelEntityObtainer<Order> businessModelEntityObtainer,
                                              IOrderRepository orderRepository,
                                              IPublicService publicService)
        {
            _finder = finder;
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _orderRepository = orderRepository;
            _publicService = publicService;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderDomainEntityDto)domainEntityDto;
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);
            
            long? reservedNumberDigit = null;
            OrderState? originalOrderState = null;
            if (entity.Id == 0)
            {
                reservedNumberDigit = _orderRepository.GenerateNextOrderUniqueNumber();
            }
            else
            {
                originalOrderState = _finder.Find(Specs.Find.ById<Order>(dto.Id)).Select(x => x.WorkflowStepId).Single();
            }

            // При операциях из UI политики безопасности НЕ игнорируем
            _publicService.Handle(new EditOrderRequest
                {
                    Entity = entity,
                    DiscountInPercents = dto.DiscountPercentChecked,
                    ReservedNumberDigit = reservedNumberDigit,
                    OriginalOrderState = originalOrderState
                });
            return entity.Id;
        }
    }
}